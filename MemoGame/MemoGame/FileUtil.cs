using System;
using System.IO;

//This is an util class, which means all of its methods will be static so that we dont need to create instance to use them
//Anything which has something in common with file reading/writing should be written here

class FileReader
{
    private static string _errorMessage;

    public static void WriteToFile(string fileName, string content)
    {
        try
        {
            File.WriteAllText(fileName, content);
        }
        catch (DirectoryNotFoundException ex)
        {
            string message = ErrorMessage + "Could not find the specified directory: '" + GetDirectoryFromFile(fileName) + "'.";
            throw new GameException(message, ex);
        }
        catch (ArgumentException ex)
        {
            string message = ErrorMessage + "The specified directory name is empty.";
            throw new GameException(message, ex);
        }
        catch (IOException ex)
        {
            string message = ErrorMessage + Environment.NewLine;
            message += ("Failed writing into: '" + fileName + "'.");
            throw new GameException(message, ex);
        }
        catch (Exception ex)
        {
            string message = ErrorMessage + Environment.NewLine;
            message += ex.Message;
            throw new GameException(message, ex);
        }
    }

    private static string GetDirectoryFromFile(string fileName)
    {
        char[] delimiers = { '\\', '/' };
        int lastDelimiterIndex = fileName.LastIndexOfAny(delimiers);

        return fileName.Remove(lastDelimiterIndex);
    }

    public static char[,] ReadFileAsMatrix(string fileName, int rows, int cols)
    {
        string imagesDirectory = @"..\..\images\";
        fileName = imagesDirectory + fileName;
        StreamReader reader;
        char[,] matrix = new char[rows, cols];

        try
        {
            reader = new StreamReader(fileName);
            using (reader)
            {
                string line = null;
                int row = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > cols || row > rows)
                    {
                        throw new GameException("File format is not correct. The file should be exactly " + rows
                            + " rows and " + cols + " characters per row.");
                    }

                    for (int i = 0; i < line.Length; i++)
                    {
                        matrix[row, i] = line[i];
                    }

                    row++;
                }
                reader.ReadLine();
            }
        }
        catch (FileNotFoundException ex)
        {
            string message = ErrorMessage + Environment.NewLine + ex.Message;
            throw new GameException(message, ex);
        }
        catch (IOException ex)
        {
            string message = ErrorMessage + Environment.NewLine;
            message += ("Failed reading '" + fileName + "'.");
            throw new GameException(message, ex);
        }
        catch (Exception ex)
        {
            string message = ErrorMessage + Environment.NewLine;
            message += ex.Message;
            throw new GameException(message, ex);
        }

        return matrix;
    }

    public static String ErrorMessage //When we want to call this util we should always set error message. 
    {                                //For example "ErrorMessage = "An error occured while trying to read the image for the cards. ""
        set { _errorMessage = value; }
        get { return _errorMessage; }
    }
}
