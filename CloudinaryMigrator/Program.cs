using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using CommandLine;
using System.Collections.Generic;
using System.IO;

namespace CloudinaryMigrator
{
    class Program
    {
        /// <summary>
        /// Subclass to provide the CLI options for the application
        /// </summary>
        public class Options
        {
            [Option('c', "cloudname",
               HelpText = "Cloudname",
               Required = true
            )]
            public string Cloudname { get; set; }

            [Option('a', "apikey",
               HelpText = "Cludinary api key",
               Required = true
            )]
            public string ApiKey { get; set; }

            [Option('s', "secret",
               HelpText = "Cludinary api secret",
               Required = true
            )]
            public string ApiSecret { get; set; }

            [Option('p', "path",
               HelpText = "path to files",
               Required = true
            )]
            public string Path { get; set; }

            [Option('x', "prefix",
               HelpText = "prefix for upload destination",
               Required = false
            )]
            public string Prefix { get; set; }

        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                string cloudname = o.Cloudname;
                string apikey = o.ApiKey;
                string apisecret = o.ApiSecret;
                string prefixDir = o.Prefix;
                DirActions dirac = new DirActions();
                string directory = o.Path;
                List<string> dirL = dirac.DirSearch(directory);
                Account acc = new Account(
                    cloudname,
                    apikey,
                    apisecret);
                Cloudinary cloudinary = new Cloudinary(acc);
                dirL.ForEach((string fileEl) =>
                {
                    Console.WriteLine(fileEl);
                    FileInfo fileInf = new FileInfo(fileEl);
                    string dirName = fileInf.DirectoryName;
                    string changeDirName = dirName.Replace(directory, "");
                    string finalDirName = prefixDir + changeDirName.Replace("\\", "/");
                    ImageUploadParams uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(fileEl),
                        Folder = finalDirName,
                        UseFilename = true,
                        UniqueFilename = false
                    };
                    try
                    {
                        cloudinary.Upload(uploadParams);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                });
            });
            Console.ReadKey();
        }

        class DirActions
        {
            public List<String> DirSearch(string sDir)
            {
                List<String> files = new List<String>();
                try
                {
                    foreach (string f in Directory.GetFiles(sDir))
                    {
                        files.Add(f);
                    }
                    foreach (string d in Directory.GetDirectories(sDir))
                    {
                        files.AddRange(DirSearch(d));
                    }
                }
                catch (System.Exception excpt)
                {
                    Console.WriteLine(excpt.Message);
                    Console.ReadKey();
                }

                return files;
            }
        }
    }

}
