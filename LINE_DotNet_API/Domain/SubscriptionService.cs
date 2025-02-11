using System;
using System.Net.Http.Headers;
using System.Text;
using LINE_DotNet_API.Dtos;
using LINE_DotNet_API.Enum;
using LINE_DotNet_API.Providers;
using Microsoft.Data.SqlClient;

namespace LINE_DotNet_API.Services
{
    public class SubscriptionService
    {
        // private readonly ConnectionStrings _connectionStrings;

        public SubscriptionService(ConnectionStrings connectionStrings)
        {
            //_connectionStrings = connectionStrings;
        }

        //public async Task<SubscriptionDto> Fetch(SubscriptionDto dto)
        //{
        //    string checkSubscriptionQuery = @"SELECT * FROM [SUBSCRIPTION] WHERE [USER_ID] = @USER_ID";
        //    using (var connection = new SqlConnection(_connectionStrings.DefaultConnection))
        //    {
        //        try
        //        {
        //            await connection.OpenAsync();

        //            // 查詢 subscription 表
        //            using (var checkCommand = new SqlCommand(checkSubscriptionQuery, connection))
        //            {
        //                checkCommand.Parameters.Add(new SqlParameter("@USER_ID", dto.USER_ID));

        //                using (var reader = await checkCommand.ExecuteReaderAsync())
        //                {
        //                    // 如果找不到記錄，返回預設值
        //                    if (!reader.HasRows)
        //                    {
        //                        return new SubscriptionDto()
        //                        {
        //                            USER_ID = dto.USER_ID,
        //                            OCEAN_POLLUTION = 0,
        //                            OIL_REPORT = 0,
        //                            SATELLITE_IMAGES = 0
        //                        };
        //                    }

        //                    // 如果找到記錄，讀取資料並返回
        //                    if (await reader.ReadAsync())
        //                    {
        //                        return new SubscriptionDto()
        //                        {
        //                            USER_ID = dto.USER_ID,
        //                            OCEAN_POLLUTION = (int)reader["OCEAN_POLLUTION"],
        //                            OIL_REPORT = (int)reader["OIL_REPORT"],
        //                            SATELLITE_IMAGES = (int)reader["SATELLITE_IMAGES"],
        //                        };
        //                    }
        //                }
        //            }

        //            // 如果到這裡，應該是邏輯上有問題，拋出例外
        //            throw new Exception("Unexpected state: user record should exist but was not found.");
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Database error", ex);
        //        }
        //    }
        //}

        public async Task Update(SubscriptionDto dto)
        {
            string checkSubscriptionQuery = @"SELECT COUNT(*) FROM [SUBSCRIPTION] WHERE [USER_ID] = @USER_ID";

            string insertSubscriptionQuery = @"
                INSERT INTO [SUBSCRIPTION] ([OCEAN_POLLUTION], [OIL_REPORT], [SATELLITE_IMAGES], [SUB_AT], [USER_ID])
                VALUES (@OCEAN_POLLUTION, @OIL_REPORT, @SATELLITE_IMAGES, GETDATE(), @USER_ID)";

            string updateSubscriptionQuery = @"
                UPDATE [SUBSCRIPTION]
                SET [OCEAN_POLLUTION] = @OCEAN_POLLUTION,
                    [OIL_REPORT] = @OIL_REPORT,
                    [SATELLITE_IMAGES] = @SATELLITE_IMAGES,
                    [SUB_AT] = GETDATE()
                WHERE [USER_ID] = @USER_ID;";

            //using (var connection = new SqlConnection(_connectionStrings.DefaultConnection))
            //{
            //    try
            //    {
            //        await connection.OpenAsync();

            //        // 檢查 userProfile 表
            //        using (var checkCommand = new SqlCommand(checkSubscriptionQuery, connection))
            //        {
            //            checkCommand.Parameters.Add(new SqlParameter("@USER_ID", dto.USER_ID));
            //            int userCount = (int)await checkCommand.ExecuteScalarAsync();

            //            // 如果 userProfile 不存在，插入新資料
            //            if (userCount == 0)
            //            {
            //                using (var insertCommand = new SqlCommand(insertSubscriptionQuery, connection))
            //                {
            //                    insertCommand.Parameters.Add(new SqlParameter("@USER_ID", dto.USER_ID));
            //                    insertCommand.Parameters.Add(new SqlParameter("@OCEAN_POLLUTION", dto.OCEAN_POLLUTION));
            //                    insertCommand.Parameters.Add(new SqlParameter("@OIL_REPORT", dto.OIL_REPORT));
            //                    insertCommand.Parameters.Add(new SqlParameter("@SATELLITE_IMAGES", dto.SATELLITE_IMAGES));
            //                    await insertCommand.ExecuteNonQueryAsync();
            //                }
            //            }
            //            else
            //            {
            //                using (var updateCommand = new SqlCommand(updateSubscriptionQuery, connection))
            //                {
            //                    updateCommand.Parameters.Add(new SqlParameter("@USER_ID", dto.USER_ID));
            //                    updateCommand.Parameters.Add(new SqlParameter("@OCEAN_POLLUTION", dto.OCEAN_POLLUTION));
            //                    updateCommand.Parameters.Add(new SqlParameter("@OIL_REPORT", dto.OIL_REPORT));
            //                    updateCommand.Parameters.Add(new SqlParameter("@SATELLITE_IMAGES", dto.SATELLITE_IMAGES));
            //                    await updateCommand.ExecuteNonQueryAsync();
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception("Database error", ex);
            //    }
            //}
        }
    }
}
