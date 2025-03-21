using Caliburn.Micro;
using ClosedXML.Excel;
using Dotnet.Gym.Message.Accounts;
using Dotnet.Gym.Message.Enums;
using ExcelDataReader;
using Ironwall.Dotnet.Libraries.Base.Services;
using Ironwall.Dotnet.Libraries.Db.Services;
using System;
using System.IO;
using System.Net.Sockets;

namespace Ironwall.Dotnet.Libraries.Db.Utils;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 3/10/2025 4:47:40 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
internal class ExcelImporter : IExcelImporter
{

    #region - Ctors -
    public ExcelImporter(ILogService log, IDbServiceForGym dbService)
    {
        _log = log;
        _dbService = dbService;
    }
    #endregion
    #region - Implementation of Interface -
    #endregion
    #region - Overrides -
    #endregion
    #region - Binding Methods -
    #endregion
    #region - Processes -

    public async Task<bool> ImportExcelToDbAsync(string filePath, CancellationToken token = default)
    {
        try
        {
            using var workbook = new XLWorkbook(filePath);
            if (workbook == null) return false;
            
            var worksheet = workbook.Worksheet(2); // 두 번째 시트
            if (worksheet == null) return false;

            int rowCount = worksheet.LastRowUsed().RowNumber();

            if (_dbService == null)
                throw new NullReferenceException($"{nameof(IDbServiceForGym)} was not instantiated...");

            var users = await _dbService.FetchUsersAsync(token);

            for (int i = 2; i <= rowCount; i++) // 헤더 제외
            {
                var row = worksheet.Row(i);
                string? locker = row.Cell(2).GetString()?.Trim();
                var locker_Color = row.Cell(2).Style.Font.FontColor;
                string? userName = row.Cell(3).GetString()?.Trim();
                // 회원명이 없으면 해당 행을 무시
                if (string.IsNullOrEmpty(userName))
                    continue;

                // 성별 변환
                string? genderStr = row.Cell(35).GetString()?.Trim();
                EnumGenderType gender = genderStr == "남" ? EnumGenderType.MALE :
                                        genderStr == "여" ? EnumGenderType.FEMALE :
                                        EnumGenderType.NONE;

                // 사용자 정보 생성
                var user = new UserModel
                {
                    UserName = userName,
                    MobilePhone = row.Cell(34).GetString()?.Trim() ?? "", // H.P.
                    Age = int.TryParse(row.Cell(36).GetString()?.Trim(), out var age) ? age : 0, // 값이 없거나 변환 실패 시 0
                    Gender = gender,
                    RegisterDate = DateTime.Today,
                    IsActive = EnumTrueFalse.True
                };

                var selectedUser = users?
                                .Where(entity => entity.UserName == user.UserName 
                                && entity.Gender == user.Gender
                                && entity.MobilePhone == user.MobilePhone
                                ).FirstOrDefault();

                // Locker 처리
                if(locker != null) 
                {
                    string? type = string.Empty;
                    if(locker_Color?.HasValue != null)
                    {
                        type = locker_Color.Color.Name;
                    }

                    user.Locker = type switch
                    {
                        "ff00b050" => new LockerModel { Locker = locker },
                        "ffff0000" => new LockerModel { ShoeLocker = locker },
                        _ => null
                    };
                }

                // 등록 기간 처리
                if (DateTime.TryParse(row.Cell(6).GetString(), out var startDate) &&
                    DateTime.TryParse(row.Cell(7).GetString(), out var endDate))
                {
                    user.ActivePeriod = new ActivePeriod
                    {
                        StartDate = startDate,
                        EndDate = endDate
                    };
                }
                else
                {
                    user.ActivePeriod = null;
                    user.IsActive = EnumTrueFalse.False;
                }


                if(selectedUser == null) 
                {
                    var ret = await _dbService.InsertUserAsync(user, token);
                    _log?.Info($"사용자(Id:{ret})가 추가되었습니다.");
                }
                else{

                    //라커비교
                    var selectedLocker = selectedUser?.Locker;
                    var userLocker = user.Locker;
                    if (selectedLocker != null && userLocker != null)
                    {
                        if (selectedLocker?.Locker != userLocker?.Locker
                            || selectedLocker?.ShoeLocker != userLocker?.ShoeLocker)
                        {
                            selectedLocker.Locker = userLocker?.Locker;
                            selectedLocker.ShoeLocker = userLocker?.ShoeLocker;
                            await _dbService.UpdateLockerAsync(selectedLocker);
                            _log?.Info($"사용자({selectedUser.Id}) 라커 업데이터...");
                        }
                    }
                    //사용기간 비교
                    var selectedActivePeriod = selectedUser?.ActivePeriod;
                    var userActivePeriod = user.ActivePeriod;
                    if (selectedActivePeriod != null && userActivePeriod != null)
                    {
                        if (selectedActivePeriod.StartDate != userActivePeriod?.StartDate
                            || selectedActivePeriod.EndDate != userActivePeriod?.EndDate)
                        {
                            selectedActivePeriod.StartDate = userActivePeriod?.StartDate;
                            selectedActivePeriod.EndDate = userActivePeriod?.EndDate;
                            await _dbService.UpdateActiveUserAsync(selectedActivePeriod);
                            _log?.Info($"사용자({selectedUser.Id}) 서비스 기간 업데이터...");
                        }
                    }
                }
                
            }
            return true;
        }
        catch (Exception ex)
        {
            _log?.Error(ex.Message);
            return false;
        }
    }
    #endregion
    #region - IHanldes -
    #endregion
    #region - Properties -
    #endregion
    #region - Attributes -
    private ILogService? _log;
    private IDbServiceForGym? _dbService;
    #endregion
}