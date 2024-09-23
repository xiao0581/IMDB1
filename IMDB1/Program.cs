using IMDB1;
using IMDB1.Model;

int lineCount = 0;
List<Title> titles = new List<Title>();
string filePath = "C:\\Users\\xiaohui\\Desktop\\4.Sql\\title.basics.tsv";

NormalInserter normalInserter = new NormalInserter();  

foreach (string line in File.ReadLines(filePath).Skip(1))
{
    if (lineCount == 50000)
    {
        break;
    }

    string[] splitLine = line.Split('\t');
    if (splitLine.Length != 9)
    {
        throw new Exception("Ikke rigtigt antal tabs! " + line);
    }

    string tconst = splitLine[0];
    string titleType = splitLine[1];
    string primaryTitle = splitLine[2];
    string originalTitle = splitLine[3];
    bool isAdult = splitLine[4] == "1";
    int startYear = splitLine[5] == "\\N" ? 0 : int.Parse(splitLine[5]);
    int endYear = splitLine[6] == "\\N" ? 0 : int.Parse(splitLine[6]);
    int runtimeMinutes = splitLine[7] == "\\N" ? 0 : int.Parse(splitLine[7]);
    string genres = splitLine[8];

    List<Genres> genresList = new List<Genres>();
    if (!string.IsNullOrEmpty(genres) && genres != "\\N")
    {
        string[] genreArray = genres.Split(',');
        foreach (string genre in genreArray)
        {
            genresList.Add(new Genres
            {
                TConst = tconst,
                Genre = genre
            });
        }
    }

    Title newTitle = new Title
    {
        TConst = tconst,
        TitleType = titleType,
        PrimaryTitle = primaryTitle,
        OriginalTitle = originalTitle,
        IsAdult = isAdult,
        StartYear = startYear,
        EndYear = endYear,
        RuntimeMinutes = runtimeMinutes,
        Genres = genresList
    };

    titles.Add(newTitle);

    // 优化：批量插入，提高性能。将 InsertData 调用移动到循环外部，减少频繁调用数据库
    if (lineCount % 1000 == 0)  // 每处理 1000 行数据后插入一次
    {
        normalInserter.InsertData(titles);
        titles.Clear();  // 清空列表，准备插入下一批数据
    }

    lineCount++;
}

// 最后一次插入，处理剩余未满 1000 行的数据
if (titles.Count > 0)
{
    normalInserter.InsertData(titles);
}