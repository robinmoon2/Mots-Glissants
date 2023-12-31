﻿using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

namespace Projet_A2_S1;
/// <summary>
/// This class is for the board of the game
/// </summary>
public class GameBoard
{

    private const string WORKING_FILE = "data/AcutalPlate.csv";
    private const string EXAMPLE_FILE = "data/Plate1.csv";
    public const string RANDOM_FILE = "data/Lettre.txt";
    private static readonly Random s_rnd = new();
    /// <summary>
    /// The board is a matrix of char that represent the board
    /// </summary>
    public char[,] Board;

    /// <summary>
    /// Build a Object GameBoard. In it the user personalized his board 
    /// It is also the **ToRead** function
    /// </summary>
    public GameBoard()
    {
        var difficultyIndex = Core.ScrollingMenuSelector("Choisissez un plateau", default, default, "Plateau par défaut", "Plateau aléatoire");
        switch (difficultyIndex.Item1)
        {
            case 0:
                string sourceFile;
                switch (difficultyIndex.Item2)
                {
                    case 0:
                        sourceFile = EXAMPLE_FILE;
                        Board = new char[8, 8];
                        GenerateExampleBoard(sourceFile);
                        SaveAndWrite();
                        break;
                    case 1:
                        var lengthmatrix = Core.ScrollingNumberSelector("Choisissez la taille du plateau", 8, 12, 8, 2);
                        Board = new char[Convert.ToInt32(lengthmatrix.Item2), Convert.ToInt32(lengthmatrix.Item2)];
                        sourceFile = RANDOM_FILE;
                        GenerateRandomBoard(sourceFile,Convert.ToInt32(lengthmatrix.Item2),Convert.ToInt32(lengthmatrix.Item2));
                        SaveAndWrite();
                        break;
                }
                break;
            default:
                Console.WriteLine("Vous avez pressé la touche echap, vous allez sortir du jeu, pressez entrée pour confirmer");
                Console.ReadKey();
                Core.ExitProgram();
                break;
        }
    }

    /// <summary>
    /// Second constructor for the Test Function.
    /// </summary>
    /// <param name="test">Boolean that indicates that is the constructor for the test project</param>
    public GameBoard(bool test)
    {
        Board = new char[8, 8];
        GenerateExampleBoard(EXAMPLE_FILE);
    }




    /// <summary>
    /// This function generate the Example Board
    /// </summary>
    /// <param name="path">the path to the file</param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="FormatException"></exception>
    void GenerateExampleBoard(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Aucun fichier à l'adresse :{path}");
        string[] lines = File.ReadAllLines(path);
        if (lines is not null)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                for (int j = 0; j < 8; j++)
                {

                    Board[i, j] = char.Parse(parts[j]);
                }
            }
        }
        else
            throw new FormatException($"Fichier vide à l'adresse :{path}");
    }
    /// <summary>
    /// Function that build a new Random Plate in a CSV file with the Lettre.txt file. 
    /// This take the number of iteration allowed per letter in the _Lettre.txt_ and add a random character for each of the case in the matrix
    /// </summary>
    /// <param name="path">The path of where is the file </param>
    /// <param name="rows">number of rows of the matrix </param>
    /// <param name="cols">number of columns of the matrix</param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="FormatException"></exception>
    void GenerateRandomBoard(string path, int rows, int cols)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Aucun fichier à l'adresse :{path}");
        var dico = new Dictionary<char, int>();
        string[] lines = File.ReadAllLines(path);
        if (lines is not null)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                dico.Add(char.Parse(parts[0]), int.Parse(parts[1]));
            }
        }
        else
            throw new FormatException($"Fichier vide à l'adresse :{path}");

        // Initialize the Board array with the given dimensions
        Board = new char[rows, cols];

        var aplhabet = "abcdefghijklmopqrstuvwxyz";
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                char randomChar = char.ToUpper(aplhabet[s_rnd.Next(aplhabet.Length-1)]);
                if (dico[randomChar] != 0)
                {
                    Board[i, j] = randomChar;
                    dico[randomChar]--;
                }
                else
                {
                    j--;
                }
            }
        }
    }

    /// <summary>
    /// Function that save the board in a CSV file it is the **ToFile** function
    /// This can write the new board into the same CSV file
    /// </summary>
    public void SaveAndWrite()
    {
        string[] file = new string[Board.GetLength(1)];
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            string line = string.Empty;
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                line += Board[i, j];
                if (j != Board.GetLength(1) - 1)
                {
                    line += ",";
                }
            }
            file[i] = line;
        }
        File.WriteAllLines(WORKING_FILE,file);
    }

    /// <summary>
    /// This function can find the word in the board
    /// It returns a dictionnary with each position as key and the character of this position in value
    /// Is is the **RechercheMot** function
    /// </summary>
    /// <param name="x"> the line of the start of the word , the first character of the word </param>
    /// <param name="y">the column of the start of the word , the first character of the word </param>
    /// <param name="word">the word that we are looking for</param>
    /// <param name="index">the number of position that are explored in the matrix that works like a int to indicate when we travel the distance of the word</param>
    /// <param name="dico">the dictionnary that is return by the function</param>
    /// <returns></returns>
    public Dictionary<(int, int), char>? GetWord(int x, int y, string word, int index = 0, Dictionary<(int, int), char>? dico = null)
    {
        dico ??= new Dictionary<(int, int), char>();
        if (word.Length == index)
            return dico;
        else if (x >= 0 && x < Board.GetLength(0) && y >= 0 && y < Board.GetLength(1))
        {
            if (char.ToLower(Board[x, y]) == (word[index]))
            {
                if (!dico.ContainsKey((x, y)))
                {
                    // if the position never had been explored, we add it to the list 
                    dico.Add((x, y), Board[x, y]);
                    if (GetWord(x - 1, y, word, index + 1, dico) != null
                        || GetWord(x + 1, y, word, index + 1, dico) != null
                        || GetWord(x, y + 1, word, index + 1, dico) != null
                        || GetWord(x, y - 1, word, index + 1, dico) != null
                        || GetWord(x - 1, y - 1, word, index + 1, dico) != null
                        || GetWord(x + 1, y + 1, word, index + 1, dico) != null
                        || GetWord(x - 1, y + 1, word, index + 1, dico) != null
                        || GetWord(x + 1, y - 1, word, index + 1, dico) != null)
                    {
                        // If we find one position around that is not null so that correspond to the following character of the word, we return dico
                        return dico;
                    }
                    //If every direction is null, we remove this position because it leads no where. 
                    dico.Remove((x, y));
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Function that update the board according to the character and the spaces.
    /// This function appear after each time the board has changed
    /// </summary>
    public void Maj_Plateau()
    {
        for (int i = 0; i < Board.GetLength(1); i++)
        {
            for (int j = 0; j < Board.GetLength(0); j++)
            {
                if (Board[i, j] == ' ')
                {
                    for (int k = i; k > 0; k--)
                    {

                        Board[k, j] = Board[k - 1, j];
                    }
                    Board[0, j] = ' ';
                }
            }
        }
    }

    /// <summary>
    /// Return the string that represent the board
    /// the override allows us to do a **Console.WriteLine(board)**
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">The file is not found </exception>
    public override string ToString()
    {
        if (!File.Exists(WORKING_FILE))
            throw new FileNotFoundException($"Aucun fichier à l'adresse :{WORKING_FILE}");
        string[] file = File.ReadAllLines(WORKING_FILE);
        var plate_string = string.Empty;
        for (int i = 0; i < file.Length; i++)
        {
            string[] parts = file[i].Split(',');
            for (int j = 0; j < Board.GetLength(0); j++)
            {
                if (parts[j] is not null)
                    plate_string += "| " + parts[j] + " |";

            }
            plate_string += "\n";
        }
        return plate_string;
    }
}