using IMDB1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;


namespace IMDB1
{
    public class NormalInserter
    {
        public void InsertData(List<Title> titles)
        {
            // 修正：移除了 Integrated Security，因为我们使用的是 SQL 登录
            string connectionString = "Server=XIAO-PC\\XIAODATA;Database=IMDB;User Id=sa;Password=12345;";

            using SqlConnection sqlConn = new SqlConnection(connectionString);
            sqlConn.Open();

            using (SqlTransaction transaction = sqlConn.BeginTransaction())  // 添加事务处理
            {
                try
                {
                    int successCount = 0;  // 添加成功插入的计数器

                    foreach (var title in titles)
                    {
                        // 先检查是否已经存在记录
                        string checkQuery = "SELECT COUNT(1) FROM Titles WHERE tconst = @Tconst";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, sqlConn, transaction))  // 将事务传递到 SqlCommand
                        {
                            checkCmd.Parameters.AddWithValue("@Tconst", title.TConst);
                            int count = (int)checkCmd.ExecuteScalar();//执行查询并返回查询结果的第一行第一列（在此例中返回记录数量）

                            if (count > 0)
                            {
                                Console.WriteLine($"Record with tconst {title.TConst} already exists. Skipping.");
                                continue;
                            }
                        }

                        string query = "INSERT INTO Titles ([tconst],[titleType],[primaryTitle],[originalTitle],[isAdult],[startYear],[endYear],[runtimeMinutes]) " +
                                       "VALUES (@Tconst, @TitleType, @PrimaryTitle, @OriginalTitle, @IsAdult, @StartYear, @EndYear, @RuntimeMinutes)";

                        using (SqlCommand sqlComm = new SqlCommand(query, sqlConn, transaction))  // 将事务传递到 SqlCommand
                        {
                            // 优化：明确指定 SqlDbType，避免 AddWithValue 自动推断导致的性能问题
                            sqlComm.Parameters.Add("@Tconst", SqlDbType.NVarChar).Value = title.TConst ?? (object)DBNull.Value;
                            sqlComm.Parameters.Add("@TitleType", SqlDbType.NVarChar).Value = title.TitleType ?? (object)DBNull.Value;
                            sqlComm.Parameters.Add("@PrimaryTitle", SqlDbType.NVarChar).Value = title.PrimaryTitle ?? (object)DBNull.Value;
                            sqlComm.Parameters.Add("@OriginalTitle", SqlDbType.NVarChar).Value = title.OriginalTitle ?? (object)DBNull.Value;
                            sqlComm.Parameters.Add("@IsAdult", SqlDbType.Bit).Value = title.IsAdult ?? (object)DBNull.Value;
                            sqlComm.Parameters.Add("@StartYear", SqlDbType.Int).Value = title.StartYear.HasValue ? (object)title.StartYear.Value : DBNull.Value;
                            sqlComm.Parameters.Add("@EndYear", SqlDbType.Int).Value = title.EndYear.HasValue ? (object)title.EndYear.Value : DBNull.Value;
                            sqlComm.Parameters.Add("@RuntimeMinutes", SqlDbType.Int).Value = title.RuntimeMinutes.HasValue ? (object)title.RuntimeMinutes.Value : DBNull.Value;

                            sqlComm.ExecuteNonQuery();
                        }

                        string genreQuery = "INSERT INTO Genres ([tconst], [genre]) VALUES (@Tconst, @Genre)";

                        foreach (var genre in title.Genres)
                        {
                            using (SqlCommand genreComm = new SqlCommand(genreQuery, sqlConn, transaction))  // 将事务传递到 SqlCommand
                            {
                                genreComm.Parameters.Add("@Tconst", SqlDbType.NVarChar).Value = title.TConst;
                                genreComm.Parameters.Add("@Genre", SqlDbType.NVarChar).Value = genre.Genre;
                                genreComm.ExecuteNonQuery();
                            }
                        }

                        successCount++;  // add success count
                    }

                    transaction.Commit();  
                    Console.WriteLine("adding data");
                    //  count the number of successful inserts
                    Console.WriteLine($"Add successful {successCount} data。");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();  
                    Console.WriteLine($"insert fail roll back: {ex.Message}");
                }
            }
        }
    }
}
