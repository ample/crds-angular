﻿using System;
using System.Collections.Generic;
using crds_angular.Models.Crossroads;
using CrossroadsStripeOnboarding.Models;
using CrossroadsStripeOnboarding.Models.Json;
using CrossroadsStripeOnboarding.Services;

namespace CrossroadsStripeOnboarding
{
    class Program
    {
        static void Main()
        {
            using (var db = new MinistryPlatformContext())
            {
                var donor = db.Donors.Find(3950248);
                Console.Write(donor);
            }

            //LoadAndImportFile();
            //CreateStripePlansAndSubscriptions();
        }

        private static void LoadAndImportFile()
        {
            var result = new KeyValuePair<Messages, StripeJsonExport>(Messages.NotRun, null);

            while (result.Key != Messages.ImportFileSuccess && result.Key != Messages.SkipImportProcess )
            {
                Console.WriteLine("Enter the exports file path/location to import.  Otherwise press S to skip this step or X to close the program: ");
                result = LoadExportFile.ReadFile(Console.ReadLine());

                switch (result.Key)
                {
                    case Messages.ReadFileSuccess:
                        result = LoadExportFile.ImportFile(result.Value);

                        if (result.Key == Messages.ImportFileSuccess)
                        {
                            Console.WriteLine("The file was processed successfully.");
                        }
                        break;
                    case Messages.FileNameRequired:
                    case Messages.FileNotFound:
                        Console.WriteLine("Please enter a valid file.");
                        break;
                    case Messages.FileContainsInvalidData:
                        Console.WriteLine("The file contains invalid data please investigate.");
                        break;
                    case Messages.Close:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private static void CreateStripePlansAndSubscriptions()
        {
            var result = Messages.NotRun;

            while (result != Messages.PlansAndSubscriptionsSuccess)
            {
                Console.WriteLine("To create Stripe Plans and Subscritions press any key to continue or X to close the Program: ");
                var command = Console.ReadLine();

                if (command != null && command.Trim().Length != 0 && command.Equals("X"))
                {
                    Environment.Exit(0);
                }

                result = StripePlansAndSubscriptions.generate();

                if (result != Messages.PlansAndSubscriptionsSuccess)
                {
                    Console.WriteLine("An error occured while creating stripe plans and subscriptions.");
                }
            }


            Console.WriteLine("The file was processed successfully. Press any key to close.");
            Console.ReadKey();
        }
    }
}