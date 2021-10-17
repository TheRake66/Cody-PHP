﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cody_PHP
{
    class Program
    {

        // Point d'entree
        static void Main(string[] args)
        {
            // --------------------------
            // Nettoie si jamais l'user l'a lancer via commande
            Console.Clear();

            // Entete
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(@"
   ______            __               ____   __  __ ____ 
  / ____/____   ____/ /__  __        / __ \ / / / // __ \
 / /    / __ \ / __  // / / /______ / /_/ // /_/ // /_/ /
/ /___ / /_/ // /_/ // /_/ //_____// ____// __  // ____/ 
\____/ \____/ \__,_/ \__, /       /_/    /_/ /_//_/      
                    /____/                               
    __     _  __                ____   __  __ ____ 
   / /    (_)/ /_ ___          / __ \ / / / // __ \
  / /    / // __// _ \ ______ / /_/ // /_/ // /_/ /
 / /___ / // /_ /  __//_____// ____// __  // ____/ 
/_____//_/ \__/ \___/       /_/    /_/ /_//_/      
                                                   
                                                 
   _____ ______ __                                             _    
  / ____|  ____/ _|                                           | |   
 | |  __| |__ | |_ _ __ __ _ _ __ ___   _____      _____  _ __| | __
 | | |_ |  __||  _| '__/ _` | '_ ` _ \ / _ \ \ /\ / / _ \| '__| |/ /
 | |__| | |   | | | | | (_| | | | | | |  __/\ V  V / (_) | |  |   <     v1.0.0.0
  \_____|_|   |_| |_|  \__,_|_| |_| |_|\___| \_/\_/ \___/|_|  |_|\_\    (gff)
");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(@"
                        Source: https://github.com/TheRake66/GFframework
                        Copyright: Thibault BUSTOS (TheRake66) - © 2021
");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"
 Garrido Fernand framework est un framework français dédié au développement du 
 site WEB en PHP/JavaScript/HTML/CSS orienté objet en MVC avec un assortiment 
 d'outils et de librairies (sécurité, base de données, formulaire, etc.).

");

            Console.ResetColor();
            // --------------------------

            while (true)
            {
                // Saut apres une commande
                Console.WriteLine();

                // Change le prompt
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Environment.UserName + "@" + Environment.MachineName);
                Console.ResetColor();
                Console.Write(":");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("~");
                Console.ResetColor();
                Console.Write("$ ");

                // Recupere les inputs, trim, et remplace les doublon d'espaces
                string input = Console.ReadLine().Trim();
                string[] split = input.Replace("  ", " ").Split(' ');
                string cmd = split[0];

                if (cmd.Length > 0)
                {
                    // Retire la commande de base des arguments
                    string[] argm = split.Skip(1).ToArray();

                    // Dispatch dans les commandes
                    switch (cmd)
                    {
                        case "new":
                            Commandes.creerProjet(argm);
                            break;

                        case "maj":
                            Commandes.verifMAJ(argm);
                            break;

                        case "com":
                            Commandes.gestComposant(argm);
                            break;

                        case "obj":
                            Commandes.gestObjet(argm);
                            break;

                        case "dl":
                            Commandes.downFile(argm);
                            break;

                        case "aide":
                            Commandes.aideCom(argm);
                            break;

                        case "cls":
                            Commandes.clearCons(argm);
                            break;

                        case "git":
                            Commandes.execGit(argm);
                            break;

                        case "rep":
                            Commandes.openRepo(argm);
                            break;

                        case "ls":
                            Commandes.listProjet(argm);
                            break;

                        case "cd":
                            Commandes.changeDir(argm);
                            break;

                        case "die":
                            Commandes.quitterApp(argm);
                            break;

                        default:
                            Console.WriteLine($"Erreur, commande '{cmd}' inconnue !");
                            break;
                    }
                }
                else
                    Console.WriteLine("Aucune commande, essayer la commande 'aide'.");
            }
        }
    }
}