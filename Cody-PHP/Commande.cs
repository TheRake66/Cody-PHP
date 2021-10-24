﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Cody_PHP.Properties;
using Newtonsoft.Json;

namespace Cody_PHP
{
    public class Commande
    {

        // Affiche l'aide
        public static void aideCom(string[] cmd)
        {
            if (cmd.Length == 0)
            {
                // Affiche l'aide
                Console.WriteLine(
@"aide                            Affiche la liste des commandes disponible.
cd [*chemin]                    Affiche ou change le dossier courant.
cls                             Nettoie la console.
com [-s|-a|-l] [nom]            Ajoute, liste, ou supprime un composant (controleur, vue, style,
                                script) avec le nom spécifié.
die                             Quitte Cody-PHP.
dl [url] [fichier]              Télécharge un fichier avec l'URL spécifiée.
exp                             Ouvre le projet dans l'explorateur de fichiers.
ls                              Affiche la liste des projets.
maj                             Met à jour Cody-PHP via le depot GitHub.
new [nom]                       Créer un nouveau projet avec le nom spécifié puis défini le dossier courant.
obj [-s|-a|-l] [nom]            Ajoute, liste, ou supprime un objet (classe dto, classe dao)
                                avec le nom spécifié.
rep                             Ouvre la dépôt GitHub de Cody-PHP.
vs                              Ouvre le projet dans Visual Studio Code.
wamp                            Lance WAMP Serveur et défini le dossier courant sur le www.

*: Argument facultatif.");
            }
            else
                Console.WriteLine("Problème, aucun argument n'est attendu !");
        }


        // Change le chemin courant
        public static void changeDir(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                string path = cmd[0];

                // Verifi si le dossier existe deja
                if (Directory.Exists(path))
                {
                    try
                    {
                        // Change le dossier
                        Directory.SetCurrentDirectory(path);
                        Console.WriteLine("Chemin changé.");
                    }
                    catch (Exception e)
                    {
                        Message.writeExcept("Impossible de changer de dossier !", e);
                    }
                }
                else
                    Console.WriteLine("Erreur, le chemin spécifié n'existe pas !");
            }
            else if (cmd.Length > 1)
                Console.WriteLine("Problème, seul un chemin est attendu.");
            else
                Console.WriteLine($"Le dossier courant est : '{Directory.GetCurrentDirectory()}'.");
        }


        // Telecharge un fichier
        public static void downFile(string[] cmd)
        {
            if (cmd.Length == 2)
            {
                // Recupere les args
                string url = cmd[0];
                string file = cmd[1];
                // Prepapre l'animation
                int x = Console.CursorLeft;
                int y = Console.CursorTop;
                int x_barre = x + 1;
                int y_barre = y + 1;
                int x_byte = x + 53;
                long total_byte = 0;
                object lk = new object(); // lock
                bool ended = false;
                
                Console.WriteLine(
@"▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄
█                                                  █
▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");

                Action<int, long, long> display_barre = (percent, receceid, total) =>
                {
                    // Progress
                    Console.SetCursorPosition(x_barre, y_barre);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    // Pas de percent / 2, le modulo est plus rapide que la division flotante
                    for (float i = 0; i < percent; i++)
                        if (i % 2 == 0) Console.Write("▓");
                    Console.ResetColor();

                    Console.SetCursorPosition(x_byte, y_barre);
                    Console.Write($"{percent}% ");
                    Message.writeIn(ConsoleColor.DarkYellow, receceid);
                    Console.Write(" octet(s) sur ");
                    Message.writeIn(ConsoleColor.DarkYellow, total);
                    Console.Write("...");
                };

                WebClient web = new WebClient();
                web.DownloadProgressChanged += (s, e) =>
                {
                    lock (lk)
                    {
                        // Progress
                        display_barre(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
                        if (total_byte == 0) total_byte = e.TotalBytesToReceive;
                        // Pour les tests
                        // dl https://launcher.mojang.com/v1/objects/a16d67e5807f57fc4e550299cf20226194497dc2/server.jar server.jar
                    }
                };

                web.DownloadFileCompleted += (s, e) =>
                {
                    lock (lk)
                    {
                        if (e.Error == null) // Si aucune exception
                        {
                            // Progress complete
                            display_barre(100, total_byte, total_byte);
                            Console.SetCursorPosition(x, y + 3);
                            Console.WriteLine("Téléchargement terminé.");
                        }
                        else
                        {
                            Console.SetCursorPosition(x, y + 3);
                            Message.writeExcept("Impossible de télécharger ce fichier !", e.Error);
                        }

                        ended = true;
                    }
                };

                // Telecharge en asyncrone
                web.DownloadFileTaskAsync(url, file);

                // Attends la fin et de delockage
                while (!ended || !Monitor.TryEnter(lk)) { }
            }
            else if (cmd.Length > 2)
                Console.WriteLine("Problème, seul l'url et le chemin du fichier sont attendus !");
            else
                Console.WriteLine("Problème, il manque l'url et le chemin du fichier !");
        }


        // Nettoire la console
        public static void clearCons(string[] cmd)
        {
            if (cmd.Length == 0)
                Console.Clear(); // Clear la console
            else
                Console.WriteLine("Problème, aucun argument n'est attendu !");
        }


        // Ouvre le depot github
        public static void openRepo(string[] cmd)
        {
            if (cmd.Length == 0)
            {
                // Ouvre dans le navigateur
                try { Process.Start("https://github.com/TheRake66/Cody-PHP"); }
                catch { }
            }
            else
                Console.WriteLine("Problème, aucun argument n'est attendu !");
        }


        // Ferme l'app
        public static void quitterApp(string[] cmd)
        {
            if (cmd.Length == 0) 
                Environment.Exit(0); // Ferme avec un code 0
            else
                Console.WriteLine("Problème, aucun argument n'est attendu !");
        }


        // Verifi les mise a jour
        public static void verifMAJ(string[] cmd)
        {
            if (cmd.Length == 0)
            {
                try
                {
                    Console.WriteLine("Vérification de la mise à jour...");

                    // Prepare un client http
                    WebClient client = new WebClient();
                    string remoteUri = "https://raw.githubusercontent.com/TheRake66/Cody-PHP/master/version";
                    string lastversion = client.DownloadString(remoteUri);

                    // Compare les version
                    if (lastversion.Equals(Program.version))
                        Console.WriteLine("Vous êtes à jour !");
                    else
                        Console.WriteLine($"La version {lastversion} est disponible, utilisez la commande 'rep' pour la télécharger !");
                }
                catch (Exception e)
                {
                    Message.writeExcept("Impossible de vérifier les mise à jour !", e);
                }
            }
            else
                Console.WriteLine("Problème, aucun argument est attendu !");
        }


        // Ouvre dans l'explorateur
        public static void openExplorer(string[] cmd)
        {
            if (cmd.Length == 0)
            {
                try
                { 
                    // Ouvre dans le navigateur
                    Process.Start("explorer.exe", Directory.GetCurrentDirectory()); 
                }
                catch (Exception e)
                {
                    Message.writeExcept("Impossible d'ouvrir l'explorateur !", e);
                }
            }
            else
                Console.WriteLine("Problème, aucun argument n'est attendu !");
        }


        // Ouvre dans vs code
        public static void openVSCode(string[] cmd)
        {
            if (cmd.Length == 0)
            {
                try
                {
                    // Ouvre dans le navigateur
                    Process.Start("code", ".");
                }
                catch (Exception e)
                {
                    Message.writeExcept("Impossible d'ouvrir Visual Studio Code !", e);
                }
            }
            else
                Console.WriteLine("Problème, aucun argument n'est attendu !");
        }


        // Gere wamp
        public static void runWamp(string[] cmd)
        {
            if (cmd.Length == 0)
            {
                try
                {
                    bool founded = false;
                    string path = "";
                    string[] folder = new string[] { "wamp64", "wamp" };
                    string[] name = new string[] { "WAMP 64-bit", "WAMP" };

                    foreach (DriveInfo drive in DriveInfo.GetDrives())
                    {
                        if (drive.DriveType == DriveType.Fixed
                            && drive.IsReady)
                        {
                            Console.Write("Lecteur : '");
                            Message.writeIn(ConsoleColor.DarkYellow, drive.Name);
                            Console.WriteLine("'...");

                            for (int i = 0; i < folder.Length; i++)
                            {
                                string f = $@"{drive.Name}{folder[i]}";
                                Console.WriteLine($"Vérification du dossier {name[i]}...");
                                if (Directory.Exists(f))
                                {
                                    path = f;
                                    founded = true;
                                    break;
                                }
                            }

                            if (founded) break;
                        }
                    }

                    if (founded)
                    {
                        // Change le dossier
                        try
                        {
                            Directory.SetCurrentDirectory($@"{path}\www");
                            Console.WriteLine("Chemin WAMP trouvé.");
                        }
                        catch (Exception e)
                        {
                            Message.writeExcept("Impossible de définir le dossier WAMP !", e);
                        }
                        // Lance wamp
                        try
                        {
                            Process.Start($@"{path}\wampmanager.exe");
                            Console.WriteLine("WAMP lancé.");
                        }
                        catch (Exception e)
                        {
                            Message.writeExcept("Impossible de lancer WAMP !", e);
                        }
                    }
                    else
                        Console.WriteLine("Aucun dossier WAMP.");
                }
                catch (Exception e)
                {
                    Message.writeExcept("Impossible de récupérer la liste des lecteur !", e);
                }
            }
            else
                Console.WriteLine("Problème, aucun argument n'est attendu !");
        }


        // ########################################################################


        // Liste les projets du dossier courant
        public static void listProjet(string[] cmd)
        {
            if (cmd.Length == 0)
            {
                try
                {
                    // Recupere tous les dossier du dossier courant
                    string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory());

                    if (dirs.Length > 0)
                    {
                        int count = 0;
                        Console.WriteLine("Nom                           Fichier   Taille         Version        Crée le                  Par");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------");

                        foreach (string dir in dirs)
                        {
                            // Si ca contient un index.php c'est un projet
                            string f = $@"{dir}\project.json";
                            if (File.Exists(f))
                            {
                                calculerProjet(dir, f);
                                count++;
                            }
                        }

                        if (count > 0)
                            Console.WriteLine("Listage terminé.");
                        else
                            Console.WriteLine("Heuuu, il n'y a aucun projet dans ce dossier...");
                    }
                    else
                        Console.WriteLine("Heuuu, il n'y a aucun dossier...");
                }
                catch (Exception e)
                {
                    Message.writeExcept("Impossible de lister les projets !", e);
                }
            }
            else
                Console.WriteLine("Problème, aucun argument n'est attendu !");
        }
        private static void calculerProjet(string dir, string file)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Message.writeIn(ConsoleColor.Magenta, Path.GetFileName(dir));

            // Calcule ne nb de fichier et la taille total
            try
            {
                long[] data = Librairie.getCountAndSizeFolder(dir);

                Console.SetCursorPosition(30, Console.CursorTop);
                Console.Write(data[0]);
                Console.SetCursorPosition(40, Console.CursorTop);
                Console.Write(data[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.SetCursorPosition(30, Console.CursorTop);
                Message.writeIn(ConsoleColor.DarkRed, "Erreur");
                Console.SetCursorPosition(40, Console.CursorTop);
                Message.writeIn(ConsoleColor.DarkRed, "Erreur");
            }

            // Recupere les info de version du projet
            try
            {
                string json = File.ReadAllText(file);
                Projet inf = JsonConvert.DeserializeObject<Projet>(json);

                Console.SetCursorPosition(55, Console.CursorTop);
                Message.writeIn(inf.version == Program.version ? ConsoleColor.Green : ConsoleColor.DarkYellow, inf.version);
                Console.SetCursorPosition(70, Console.CursorTop);
                Console.Write(inf.creation.ToString());
                Console.SetCursorPosition(95, Console.CursorTop);
                Console.Write(inf.createur);
            }
            catch
            {

                Console.SetCursorPosition(55, Console.CursorTop);
                Message.writeIn(ConsoleColor.DarkRed, "Erreur");
                Console.SetCursorPosition(70, Console.CursorTop);
                Message.writeIn(ConsoleColor.DarkRed, "Erreur");
                Console.SetCursorPosition(95, Console.CursorTop);
                Message.writeIn(ConsoleColor.DarkRed, "Erreur");
            }

            Console.WriteLine();
        }

        // Creer un nouveau projet
        public static void creerProjet(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                // Verifi si projet existe deja
                string name = cmd[0];

                if (!Directory.Exists(name))
                    creerDossierProjet(name);
                else
                    Console.WriteLine("Heuuu, le projet existe déjà, ou un dossier...");
            }
            else if (cmd.Length > 1)
                Console.WriteLine("Problème, seul le nom du nouveau projet est attendu !");
            else
                Console.WriteLine("Problème, il manque le nom du nouveau projet !");
        }
        private static void creerDossierProjet(string nom)
        {
            try
            {
                // Creer le dossier du projet
                Console.WriteLine("Création du dossier du projet...");
                Directory.CreateDirectory(nom);

                extractionArchiveProject(nom);
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de créer le dossier du projet !", e);
            }
        }
        private static void extractionArchiveProject(string nom)
        {
            try
            {
                // Extrait l'archive des ressouces
                Console.WriteLine("Extraction de l'archive...");
                string zip = Path.Combine(nom, "base_projet.zip");
                File.WriteAllBytes(zip, Resources.base_projet);

                parcoursArchiveProjet(zip, nom);
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'extraire l'archive !", e);
            }
        }
        private static void parcoursArchiveProjet(string zip, string nom)
        {
            try
            {
                // Ouvre l'archive
                using (ZipArchive arc = ZipFile.OpenRead(zip))
                {
                    // Parcour chaque entree
                    foreach (ZipArchiveEntry ent in arc.Entries)
                    {
                        string path = Path.Combine(nom, ent.FullName); // projet\entry

                        // Si c'est un dossier
                        if (ent.Name == "")
                            extraireDossierProjet(path, ent.FullName);
                        // Si c'est un fichier
                        else
                            extraireFichierProjet(ent, path, ent.FullName, nom);
                    }
                }

                supprimerArchiveProjet(zip);
                creerJsonProject(nom);
                changerDossierProjet(nom);

                Console.WriteLine("Le projet a été crée.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'extraire l'archive !", e);
            }
        }
        private static void extraireDossierProjet(string path, string file)
        {
            try
            {
                // Creer le dossier
                Directory.CreateDirectory(path);

                Console.Write("Dossier : '");
                Message.writeIn(ConsoleColor.Magenta, file);
                Console.WriteLine("'. Dossier ajouté.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'ajouter le dossier !", e);
            }
        }
        private static void extraireFichierProjet(ZipArchiveEntry ent, string path, string file, string name)
        {
            try
            {
                // Extrait le fichier de l'archive
                ent.ExtractToFile(path);
                Console.Write("Fichier : '");
                Message.writeIn(ConsoleColor.DarkGreen, file);
                Console.Write("'. Extraction du fichier, ");
                Message.writeIn(ConsoleColor.DarkYellow, new FileInfo(path).Length);
                Console.WriteLine(" octet(s) au total.");

                // Fichiers ou l'on rajoute le nom
                string[] toedit = new string[]
                {
                    ".php",
                    ".js",
                    ".json",
                    ".less"
                };

                if (toedit.Contains(Path.GetExtension(file)))
                {
                    try
                    {
                        Console.WriteLine("Édition du fichier...");
                        // Modifie le fichier
                        File.WriteAllText(path, File.ReadAllText(path).Replace("{PROJECT_NAME}", name));
                        Console.WriteLine("Édition du fichier terminé.");
                    }
                    catch (Exception e)
                    {
                        Message.writeExcept("Impossible d'éditer le fichier !", e);
                    }
                }
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'extraire le fichier !", e);
            }
        }
        private static void supprimerArchiveProjet(string zip)
        {
            try
            {
                // Supprime l'archive
                Console.WriteLine("Suppression de l'archive...");
                File.Delete(zip);
                Console.WriteLine("Archive supprimée.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de supprimer l'archive !", e);
            }
        }
        private static void creerJsonProject(string name)
        {

            try
            {
                // Creer le cody json
                Console.WriteLine("Création du fichier d'information du projet...");

                Projet inf = new Projet();
                inf.createur = Environment.UserName;
                inf.version = Program.version;
                inf.creation = DateTime.Now;

                string json = JsonConvert.SerializeObject(inf, Formatting.Indented);
                File.WriteAllText(Path.Combine(name, "project.json"), json);

                Console.WriteLine("Fichier d'information crée.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de créer le fichier d'information du projet !", e);
            }
        }
        private static void changerDossierProjet(string name)
        {

            try
            {
                // Change le dossier courant
                Console.WriteLine("Changement du dossier courant...");

                Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), name));

                Console.WriteLine("Dossier courant changé.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de changer le dossier courant !", e);
            }
        }


        // ########################################################################


        // Gere les objets
        public static void gestObjet(string[] cmd)
        {
            if (cmd.Length == 1 || cmd.Length == 2)
            {
                // Si le projet existe
                if (File.Exists("project.json"))
                {
                    try
                    {
                        string json = File.ReadAllText("project.json");
                        Projet inf = JsonConvert.DeserializeObject<Projet>(json);
                        bool continu = true;

                        // Conflit de version
                        if (inf.version != Program.version)
                        {
                            Console.Write("Attention, ce projet est fait pour fonctionner avec la version ");
                            Message.writeIn(ConsoleColor.DarkYellow, inf.version);
                            Console.WriteLine(" de Cody-PHP.");
                            Console.Write("Vous êtes en version ");
                            Message.writeIn(ConsoleColor.Green, Program.version);
                            Console.WriteLine(", cela pourrait créer des problèmes de compatibilité, voulez vous continuer ?");

                            string rep = null;
                            do
                            {
                                Console.Write("(oui/non) : ");
                                rep = Console.ReadLine().Trim().ToLower();
                            }
                            while (rep != "oui" && rep != "non");
                            continu = rep == "oui";
                        }

                        if (continu)
                        {
                            switch (cmd[0].ToLower())
                            {
                                case "-l":
                                    listerObjet();
                                    break;

                                case "-s":
                                    if (cmd.Length == 2) supprimerObjet(cmd[1]);
                                    else Console.WriteLine("Il manque le nom de l'objet !");
                                    break;

                                case "-a":
                                    if (cmd.Length == 2) ajouterObjet(cmd[1]);
                                    else Console.WriteLine("Il manque le nom de l'objet !");
                                    break;

                                default:
                                    Console.WriteLine("Le type d'action est invalide !");
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Message.writeExcept("Impossible de lire le fichier d'information de Cody-PHP !", e);
                    }
                }
                else
                    Console.WriteLine("Heuu, le dossier courant n'est pas un projet de Cody-PHP...");
            }
            else if (cmd.Length > 2)
                Console.WriteLine("Problème, trop d'arguments ont été données !");
            else
                Console.WriteLine("Problème, il manque le type d'action ou le nom de l'objet !");
        }

        // Ajoute un objet
        private static void ajouterObjet(string nom)
        {
            bool continu = true;
            List<Objet> objs = new List<Objet>();

            if (File.Exists("object.json"))
            {
                try
                {
                    string json = File.ReadAllText("object.json");

                    if (json != "")
                    {
                        objs = JsonConvert.DeserializeObject<List<Objet>>(json);

                        foreach (Objet obj in objs)
                        {
                            if (obj.nom == nom)
                            {
                                Console.WriteLine("Heuuu, l'objet existe déjà...");
                                continu = false;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Impossible de lire la liste des objets existant !", e);
                    continu = false;
                }
            }

            if (continu) extractionArchiveObjet(objs, nom);
        }
        private static void extractionArchiveObjet(List<Objet> objs, string nom)
        {
            try
            {
                // Extrait l'archive des ressouces
                Console.WriteLine("Extraction de l'archive...");
                string zip = "base_objet.zip";
                File.WriteAllBytes(zip, Resources.base_objet);

                parcoursArchiveObjet(objs, zip, nom);
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'extraire l'archive !", e);
            }
        }
        private static void parcoursArchiveObjet(List<Objet> objs, string zip, string nom)
        {
            try
            {
                string[] spt = nom.Split(Path.DirectorySeparatorChar);
                string namespce = ""; // \Namepace\Namespace
                string objlow = ""; // obj
                string objup = ""; // Obj
                string nomlow = nom.ToLower(); // \namepace\namespace\obj
                List<string> paths = new List<string>();

                for (int i = 0; i < spt.Length - 1; i++)
                {
                    string n = spt[i];
                    namespce += $@"\{n.Substring(0, 1).ToUpper()}";
                    if (n.Length > 1) namespce += n.Substring(1).ToLower();
                }
                objlow = spt[spt.Length - 1].ToLower();
                objup = objlow.Substring(0, 1).ToUpper();
                if (objlow.Length > 1) objup += objlow.Substring(1);

                // Ouvre l'archive
                using (ZipArchive arc = ZipFile.OpenRead(zip))
                {
                    // Parcour chaque entree
                    foreach (ZipArchiveEntry ent in arc.Entries)
                    {
                        // Si c'est un fichier
                        if (ent.Name != "")
                            extraireFichierObjet(ent, ref paths, nomlow, namespce, objlow, objup);
                    }
                }

                supprimerArchiveObjet(zip);
                ajouterJsonObjet(objs, paths, nom);

                Console.WriteLine("L'objet a été crée.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'extraire l'archive !", e);
            }
        }
        private static void extraireFichierObjet(ZipArchiveEntry ent, ref List<string> paths, string nomlow, string namespce, string objlow, string objup)
        {
            try
            {
                // modele\dto\*.php --> modele\dto\namepace\namespace\obh.php
                string file = Path.Combine(Path.GetDirectoryName(ent.FullName), nomlow) + Path.GetExtension(ent.Name);
                string path = Path.GetDirectoryName(file);

                bool continu = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                        Console.Write("Dossier : '");
                        Message.writeIn(ConsoleColor.Magenta, path);
                        Console.WriteLine("'. Dossier ajouté.");
                    }
                    catch (Exception e)
                    {
                        Message.writeExcept("Impossible d'ajouter le(s) dossier(s) !", e);
                        continu = false;
                    }
                }

                if (continu)
                {
                    // Extrait le fichier de l'archive
                    ent.ExtractToFile(file);
                    paths.Add(file);
                    Console.Write("Fichier : '");
                    Message.writeIn(ConsoleColor.DarkGreen, file);
                    Console.WriteLine("'. Extraction du terminé.");

                    try
                    {
                        Console.WriteLine("Édition du fichier...");

                        // Modifie le fichier
                        string content = File.ReadAllText(file)
                            .Replace("{NAMESPACE}", namespce)
                            .Replace("{NAME_UPPER}", objup)
                            .Replace("{NAME_LOWER}", objlow);
                        File.WriteAllText(file, content);

                        Console.WriteLine("Édition du fichier terminé.");
                    }
                    catch (Exception e)
                    {
                        Message.writeExcept("Impossible d'éditer le fichier !", e);
                    }
                }
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'extraire le fichier !", e);
            }
        }
        private static void supprimerArchiveObjet(string zip)
        {
            try
            {
                // Supprime l'archive
                Console.WriteLine("Suppression de l'archive...");
                File.Delete(zip);
                Console.WriteLine("Archive supprimée.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de supprimer l'archive !", e);
            }
        }
        private static void ajouterJsonObjet(List<Objet> objs, List<string> paths, string nom)
        {
            try
            {
                Console.WriteLine("Indexation de l'objet...");
                
                Objet obj = new Objet();
                obj.nom = nom;
                obj.createur = Environment.UserName;
                obj.creation = DateTime.Now;
                obj.chemins = paths;
                objs.Add(obj);

                string json = JsonConvert.SerializeObject(objs, Formatting.Indented);
                File.WriteAllText("object.json", json);

                Console.WriteLine("Objet indexé.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'indexé l'objet !", e);
            }
        }

        // Suprime un objet
        private static void supprimerObjet(string nom)
        {
            if (File.Exists("object.json"))
            {
                try
                {
                    string json = File.ReadAllText("object.json");

                    if (json != "")
                    {
                        List<Objet> objs = JsonConvert.DeserializeObject<List<Objet>>(json);
                        parcoursPourSupprimerObjet(objs, nom);
                    }
                    else
                    {
                        Console.WriteLine("Heuuu, aucun objet n'est indexé...");
                    }
                }
                catch (Exception e)
                {
                    Message.writeExcept("Impossible de lire la liste des objets existant !", e);
                }
            }
            else
                Console.WriteLine("Heuuu, aucune liste d'objet n'a été trouvée...");
        }
        private static void parcoursPourSupprimerObjet(List<Objet> objs, string nom)
        {
            bool trouve = false;
            bool continu = true;

            foreach (Objet obj in objs)
            {
                if (obj.nom == nom)
                {
                    objs.Remove(obj);
                    trouve = true;
                    foreach (string file in obj.chemins)
                    {
                        if (File.Exists(file))
                        {
                            supprimerFichierObjet(file, ref continu);
                        }
                        else
                        {
                            Console.Write("Le fichier '");
                            Message.writeIn(ConsoleColor.DarkYellow, file);
                            Console.WriteLine("' est indexé mais est introuvable !");
                        }
                    }
                    break;
                }
            }

            if (trouve)
            {
                if (continu)
                    supprimerJsonObjet(objs);
                else
                    Console.WriteLine("L'objet a été partiellement supprimé.");
            }
            else
                Console.WriteLine("L'objet n'existe pas !");
        }
        private static void supprimerFichierObjet(string file, ref bool continu)
        {
            try
            {
                Console.Write("Suppression du fichier '");
                Message.writeIn(ConsoleColor.Red, file);
                Console.WriteLine("'...");
                File.Delete(file);
                Console.WriteLine("Fichier supprimé.");

                string folder = Path.GetDirectoryName(file);
                if (Directory.GetFiles(folder).Length == 0 &&
                    Path.GetDirectoryName(Directory.GetCurrentDirectory()) != Path.GetDirectoryName(folder))
                {
                    supprimerDossierObjet(folder, ref continu);
                }
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de supprimer le fichier !", e);
                continu = false;
            }
        }
        private static void supprimerDossierObjet(string folder, ref bool continu)
        {
            try
            {
                Console.Write("Suppression du dossier '");
                Message.writeIn(ConsoleColor.Magenta, folder);
                Console.WriteLine("'...");
                Directory.Delete(folder);
                Console.WriteLine("Dossier supprimé.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de supprimer le dossier !", e);
                continu = false;
            }
        }
        private static void supprimerJsonObjet(List<Objet> objs)
        {
            try
            {
                Console.WriteLine("Désindexation de l'objet...");

                string json = JsonConvert.SerializeObject(objs, Formatting.Indented);
                File.WriteAllText("object.json", json);

                Console.WriteLine("Objet désindexé.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de désindexé l'objet !", e);
            }
        }

        // Liste les objets
        private static void listerObjet()
        {
            if (File.Exists("object.json"))
            {
                try
                {
                    string json = File.ReadAllText("object.json");

                    if (json != "")
                    {
                        List<Objet> objs = JsonConvert.DeserializeObject<List<Objet>>(json);

                        Console.WriteLine("Nom                      Fichier        Crée le                  Par");
                        Console.WriteLine("-------------------------------------------------------------------------------------");

                        int count = 0;
                        foreach (Objet obj in objs)
                        {
                            affichierUnObjet(obj);
                            count++;
                        }

                        if (count > 0)
                            Console.WriteLine("Listage terminé.");
                        else
                            Console.WriteLine("Heuuu, il n'y a aucun objet dans ce projet...");
                    }
                    else
                    {
                        Console.WriteLine("Heuuu, aucun objet n'est indexé...");
                    }
                }
                catch (Exception e)
                {
                    Message.writeExcept("Impossible de lire la liste des objets existant !", e);
                }
            }
            else
                Console.WriteLine("Heuuu, aucune liste d'objet a été trouvée...");
        }
        private static void affichierUnObjet(Objet obj)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Message.writeIn(ConsoleColor.Magenta, obj.nom);

            int count2 = 0;
            foreach (string file in obj.chemins)
                if (File.Exists(file)) count2++;

            Console.SetCursorPosition(25, Console.CursorTop);
            if (count2 == obj.chemins.Count)
                Console.Write(obj.chemins.Count);
            else
                Message.writeIn(ConsoleColor.DarkRed, $"{count2} ({obj.chemins.Count})");


            Console.SetCursorPosition(40, Console.CursorTop);
            Console.Write(obj.creation.ToString());
            Console.SetCursorPosition(65, Console.CursorTop);
            Console.WriteLine(obj.createur);
        }


        // ########################################################################


        // Gere les composants
        public static void gestComposant(string[] cmd)
        {
            if (cmd.Length == 1 || cmd.Length == 2)
            {
                // Si le projet existe
                if (File.Exists("project.json"))
                {
                    try
                    {
                        string json = File.ReadAllText("project.json");
                        Projet inf = JsonConvert.DeserializeObject<Projet>(json);
                        bool continu = true;

                        // Conflit de version
                        if (inf.version != Program.version)
                        {
                            Console.Write("Attention, ce projet est fait pour fonctionner avec la version ");
                            Message.writeIn(ConsoleColor.DarkYellow, inf.version);
                            Console.WriteLine(" de Cody-PHP.");
                            Console.Write("Vous êtes en version ");
                            Message.writeIn(ConsoleColor.Green, Program.version);
                            Console.WriteLine(", cela pourrait créer des problèmes de compatibilité, voulez vous continuer ?");

                            string rep = null;
                            do
                            {
                                Console.Write("(oui/non) : ");
                                rep = Console.ReadLine().Trim().ToLower();
                            }
                            while (rep != "oui" && rep != "non");
                            continu = rep == "oui";
                        }

                        if (continu)
                        {
                            switch (cmd[0].ToLower())
                            {
                                case "-l":
                                    listerComposant();
                                    break;

                                case "-s":
                                    if (cmd.Length == 2) supprimerComposant(cmd[1]);
                                    else Console.WriteLine("Il manque le nom de l'objet !");
                                    break;

                                case "-a":
                                    if (cmd.Length == 2) ajouterComposant(cmd[1]);
                                    else Console.WriteLine("Il manque le nom de l'objet !");
                                    break;

                                default:
                                    Console.WriteLine("Le type d'action est invalide !");
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Message.writeExcept("Impossible de lire le fichier d'information de Cody-PHP !", e);
                    }
                }
                else
                    Console.WriteLine("Heuu, le dossier courant n'est pas un projet de Cody-PHP...");
            }
            else if (cmd.Length > 2)
                Console.WriteLine("Problème, trop d'arguments ont été données !");
            else
                Console.WriteLine("Problème, il manque le type d'action ou le nom du composant !");
        }

        // Ajoute un composant
        private static void ajouterComposant(string nom)
        {
            bool continu = true;
            List<Composant> comps = new List<Composant>();

            if (File.Exists("component.json"))
            {
                try
                {
                    string json = File.ReadAllText("component.json");

                    if (json != "")
                    {
                        comps = JsonConvert.DeserializeObject<List<Composant>>(json);

                        foreach (Composant comp in comps)
                        {
                            if (comp.nom == nom)
                            {
                                Console.WriteLine("Heuuu, le composant existe déjà...");
                                continu = false;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Impossible de lire la liste des composants existant !", e);
                    continu = false;
                }
            }

            if (continu) extractionArchiveComposant(comps, nom);
        }
        private static void extractionArchiveComposant(List<Composant> comps, string nom)
        {
            try
            {
                // Extrait l'archive des ressouces
                Console.WriteLine("Extraction de l'archive...");
                string zip = "base_composant.zip";
                File.WriteAllBytes(zip, Resources.base_composant);

                parcoursArchiveComposant(comps, zip, nom);
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'extraire l'archive !", e);
            }
        }
        private static void parcoursArchiveComposant(List<Composant> comps, string zip, string nom)
        {
            try
            {
                string[] spt = nom.Split(Path.DirectorySeparatorChar);
                string namespce = ""; // \Namepace\Namespace
                string comlow = ""; // comp
                string comup = ""; // Comp
                string nomlow = nom.ToLower(); // \namepace\namespace\comp
                List<string> paths = new List<string>();

                for (int i = 0; i < spt.Length - 1; i++)
                {
                    string n = spt[i];
                    namespce += $@"\{n.Substring(0, 1).ToUpper()}";
                    if (n.Length > 1) namespce += n.Substring(1).ToLower();
                }
                comlow = spt[spt.Length - 1].ToLower();
                comup = comlow.Substring(0, 1).ToUpper();
                if (comlow.Length > 1) comup += comlow.Substring(1);

                // Ouvre l'archive
                using (ZipArchive arc = ZipFile.OpenRead(zip))
                {
                    // Parcour chaque entree
                    foreach (ZipArchiveEntry ent in arc.Entries)
                    {
                        // Si c'est un fichier
                        if (ent.Name != "")
                            extraireFichierComposant(ent, ref paths, nomlow, namespce, comlow, comup);
                    }
                }

                supprimerArchiveComposant(zip);
                ajouterJsonComposant(comps, paths, nom);

                Console.WriteLine("Le composant a été crée.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'extraire l'archive !", e);
            }
        }
        private static void extraireFichierComposant(ZipArchiveEntry ent, ref List<string> paths, string nomlow, string namespce, string comlow, string comup)
        {
            try
            {
                // modele\dto\*.php --> modele\dto\namepace\namespace\obh.php
                string file = Path.Combine(Path.GetDirectoryName(ent.FullName), nomlow) + Path.GetExtension(ent.Name);
                string path = Path.GetDirectoryName(file);

                bool continu = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                        Console.Write("Dossier : '");
                        Message.writeIn(ConsoleColor.Magenta, path);
                        Console.WriteLine("'. Dossier ajouté.");
                    }
                    catch (Exception e)
                    {
                        Message.writeExcept("Impossible d'ajouter le(s) dossier(s) !", e);
                        continu = false;
                    }
                }

                if (continu)
                {
                    // Extrait le fichier de l'archive
                    ent.ExtractToFile(file);
                    paths.Add(file);
                    Console.Write("Fichier : '");
                    Message.writeIn(ConsoleColor.DarkGreen, file);
                    Console.WriteLine("'. Extraction du terminé.");

                    try
                    {
                        Console.WriteLine("Édition du fichier...");

                        // Modifie le fichier
                        string content = File.ReadAllText(file)
                            .Replace("{NAMESPACE}", namespce)
                            .Replace("{NAME_UPPER}", comup)
                            .Replace("{PATH}", nomlow.Replace('\\', '/'))
                            .Replace("{NAME_LOWER}", comlow);
                        File.WriteAllText(file, content);

                        Console.WriteLine("Édition du fichier terminé.");
                    }
                    catch (Exception e)
                    {
                        Message.writeExcept("Impossible d'éditer le fichier !", e);
                    }
                }
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'extraire le fichier !", e);
            }
        }
        private static void supprimerArchiveComposant(string zip)
        {
            try
            {
                // Supprime l'archive
                Console.WriteLine("Suppression de l'archive...");
                File.Delete(zip);
                Console.WriteLine("Archive supprimée.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de supprimer l'archive !", e);
            }
        }
        private static void ajouterJsonComposant(List<Composant> comps, List<string> paths, string nom)
        {
            try
            {
                Console.WriteLine("Indexation de lu composant...");

                Composant comp = new Composant();
                comp.nom = nom;
                comp.createur = Environment.UserName;
                comp.creation = DateTime.Now;
                comp.chemins = paths;
                comps.Add(comp);

                string json = JsonConvert.SerializeObject(comps, Formatting.Indented);
                File.WriteAllText("component.json", json);

                Console.WriteLine("Composant indexé.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible d'indexé le composant !", e);
            }
        }

        // Suprime un composant
        private static void supprimerComposant(string nom)
        {
            if (File.Exists("component.json"))
            {
                try
                {
                    string json = File.ReadAllText("component.json");

                    if (json != "")
                    {
                        List<Composant> comps = JsonConvert.DeserializeObject<List<Composant>>(json);
                        parcoursPourSupprimerComposant(comps, nom);
                    }
                    else
                    {
                        Console.WriteLine("Heuuu, aucun composant n'est indexé...");
                    }
                }
                catch (Exception e)
                {
                    Message.writeExcept("Impossible de lire la liste des composant existant !", e);
                }
            }
            else
                Console.WriteLine("Heuuu, aucune liste de composant n'a été trouvée...");
        }
        private static void parcoursPourSupprimerComposant(List<Composant> comps, string nom)
        {
            bool trouve = false;
            bool continu = true;

            foreach (Composant comp in comps)
            {
                if (comp.nom == nom)
                {
                    comps.Remove(comp);
                    trouve = true;
                    foreach (string file in comp.chemins)
                    {
                        if (File.Exists(file))
                        {
                            supprimerFichierComposant(file, ref continu);
                        }
                        else
                        {
                            Console.Write("Le fichier '");
                            Message.writeIn(ConsoleColor.DarkYellow, file);
                            Console.WriteLine("' est indexé mais est introuvable !");
                        }
                    }
                    break;
                }
            }

            if (trouve)
            {
                if (continu)
                    supprimerJsonComposant(comps);
                else
                    Console.WriteLine("Le composant a été partiellement supprimé.");
            }
            else
                Console.WriteLine("Le composant n'existe pas !");
        }
        private static void supprimerFichierComposant(string file, ref bool continu)
        {
            try
            {
                Console.Write("Suppression du fichier '");
                Message.writeIn(ConsoleColor.Red, file);
                Console.WriteLine("'...");
                File.Delete(file);
                Console.WriteLine("Fichier supprimé.");

                string folder = Path.GetDirectoryName(file);
                if (Directory.GetFiles(folder).Length == 0 &&
                    Path.GetDirectoryName(Directory.GetCurrentDirectory()) != Path.GetDirectoryName(folder))
                {
                    supprimerDossierComposant(folder, ref continu);
                }
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de supprimer le fichier !", e);
                continu = false;
            }
        }
        private static void supprimerDossierComposant(string folder, ref bool continu)
        {
            try
            {
                Console.Write("Suppression du dossier '");
                Message.writeIn(ConsoleColor.Magenta, folder);
                Console.WriteLine("'...");
                Directory.Delete(folder);
                Console.WriteLine("Dossier supprimé.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de supprimer le dossier !", e);
                continu = false;
            }
        }
        private static void supprimerJsonComposant(List<Composant> comps)
        {
            try
            {
                Console.WriteLine("Désindexation du composant...");

                string json = JsonConvert.SerializeObject(comps, Formatting.Indented);
                File.WriteAllText("component.json", json);

                Console.WriteLine("Composant désindexé.");
            }
            catch (Exception e)
            {
                Message.writeExcept("Impossible de désindexé le composant !", e);
            }
        }

        // Liste les composants
        private static void listerComposant()
        {
            if (File.Exists("component.json"))
            {
                try
                {
                    string json = File.ReadAllText("component.json");

                    if (json != "")
                    {
                        List<Composant> comps = JsonConvert.DeserializeObject<List<Composant>>(json);

                        Console.WriteLine("Nom                      Fichier        Crée le                  Par");
                        Console.WriteLine("-------------------------------------------------------------------------------------");

                        int count = 0;
                        foreach (Composant comp in comps)
                        {
                            affichierUnComposant(comp);
                            count++;
                        }

                        if (count > 0)
                            Console.WriteLine("Listage terminé.");
                        else
                            Console.WriteLine("Heuuu, il n'y a aucun composant dans ce projet...");
                    }
                    else
                    {
                        Console.WriteLine("Heuuu, aucun composant n'est indexé...");
                    }
                }
                catch (Exception e)
                {
                    Message.writeExcept("Impossible de lire la liste des composants existant !", e);
                }
            }
            else
                Console.WriteLine("Heuuu, aucune liste de composant a été trouvée...");
        }
        private static void affichierUnComposant(Composant comp)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Message.writeIn(ConsoleColor.Magenta, comp.nom);

            int count2 = 0;
            foreach (string file in comp.chemins)
                if (File.Exists(file)) count2++;

            Console.SetCursorPosition(25, Console.CursorTop);
            if (count2 == comp.chemins.Count)
                Console.Write(comp.chemins.Count);
            else
                Message.writeIn(ConsoleColor.DarkRed, $"{count2} ({comp.chemins.Count})");


            Console.SetCursorPosition(40, Console.CursorTop);
            Console.Write(comp.creation.ToString());
            Console.SetCursorPosition(65, Console.CursorTop);
            Console.WriteLine(comp.createur);
        }

    }
}