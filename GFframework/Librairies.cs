﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GFFramework
{
    public class Librairies
    {

        public static long[] getCountAndSizeFolder(string path)
        {
            long[] data = { 0, 0 };

            foreach (string f in Directory.GetFiles(path))
            {
                data[0]++;

                FileInfo info = new FileInfo(f);
                data[1] += info.Length;
            }

            foreach (string d in Directory.GetDirectories(path))
            {
                long[] recursive = getCountAndSizeFolder(d);
                data[0] += recursive[0];
                data[1] += recursive[1];
            }

            return data;
        }

    }
}
