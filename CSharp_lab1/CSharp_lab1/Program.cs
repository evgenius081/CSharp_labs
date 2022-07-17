using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace ExtensionMethods
{
    public static class  DirectoryInfoExtensions
    {
        public static DateTime GetOldestFile(this DirectoryInfo di, DateTime oldest)
        {   
            DateTime current = Directory.GetCreationTime(di.FullName);
            if (current < oldest)
            {
                oldest = current;
            }
            var dirs = Directory.GetDirectories(di.FullName);
            foreach (var dir in dirs){
                DirectoryInfo dis = new DirectoryInfo(dir);
                DateTime dt = dis.GetOldestFile(oldest);
                if (dt < oldest)
                {
                    oldest = dt;
                }
            }

            string[] files = Directory.GetFiles(di.FullName);
            foreach (var file in files)
            {
                DateTime dt = File.GetCreationTime(file);
                if (dt < oldest)
                {
                    oldest = dt;
                }
            }

            return oldest;
        }
    }

    public static class FileSystemInfoExtension
    {
        public static string Rash(this FileSystemInfo fsi)
        {
            StringBuilder ret = new StringBuilder("----");
            ret[0] = ((fsi.Attributes & FileAttributes.ReadOnly) != 0) ? 'r' : '-';
            ret[1] = ((fsi.Attributes & FileAttributes.Archive) != 0) ? 'a' : '-';
            ret[2] = ((fsi.Attributes & FileAttributes.System) != 0) ? 's' : '-';
            ret[3] = ((fsi.Attributes & FileAttributes.Hidden) != 0) ? 'h' : '-';
            return ret.ToString();
        }
    }
}

namespace PT_lab7{
    using ExtensionMethods;

    [Serializable]
    public class StrComp : IComparer<string>
    {
        public int Compare(string? first, string? second)
        {
            if (second != null && first != null && first.Length > second.Length)
            {
                return 1;
            }
            else if (second != null && first != null && first.Length == second.Length)
            {
                return string.Compare(first, second, StringComparison.CurrentCulture);
            }
            return -1;
        }
    }

    class Prog{
        static void Serialize(SortedList<string, long> sl, string filename)  
        {
            Stream ms = File.OpenWrite(filename);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, sl);  
            ms.Flush();  
            ms.Close();  
            ms.Dispose();  
        } 
        
        static SortedList<string, long> Deserialize(string filename)  
        {  
            //Format the object as Binary  
            BinaryFormatter formatter = new BinaryFormatter();  
   
            //Reading the file from the server  
            FileStream fs = File.Open(filename, FileMode.Open);  
   
            object obj = formatter.Deserialize(fs);  
            SortedList<string, long> sl  = (SortedList<string, long>)obj;  
            fs.Flush();  
            fs.Close();  
            fs.Dispose();
            return sl;
        }
        
        public static void Main(string[] args)
        {
            DisplayFolder(args[0], 0);
            DirectoryInfo di = new DirectoryInfo(args[0]);
            DateTime dt = di.GetOldestFile(DateTime.Now);
            Console.WriteLine("Oldest file: {0}\n", dt);
            SortedList<string, long> sl = new SortedList<string, long>(new StrComp());
            var dirs = Directory.GetDirectories(args[0]);
            foreach (var dir in dirs){
                sl.Add(dir, (Convert.ToInt64(Directory.GetDirectories(dir).Length+Directory.GetFiles(dir).Length)));
            }
            var files = Directory.GetFiles(args[0]);
            foreach (var file in files){
                FileInfo fi = new FileInfo(file);
                sl.Add(file, fi.Length);
            }
            string pth = @"D:\Test.bin";  
            Serialize(sl, pth);
            SortedList<string, long> readSl = Deserialize(pth);
            foreach (KeyValuePair<string, long> kvp in readSl)
            {
                Console.WriteLine("{0} -> {1}", kvp.Key, kvp.Value);
            }
        }

        static void DisplayTabs(int level)
        {
            for (int i = 0; i < level; i++)
            {
                Console.Write("\t");
            }
        }

        static void DisplayFolder(string path, int level)
        {
            DisplayTabs(level);
            DirectoryInfo di = new DirectoryInfo(path);
            Console.WriteLine("{0} ({1}) {2}", path, (Directory.GetDirectories(path).Length + Directory.GetFiles(path).Length), di.Rash());
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs){
                DisplayFolder(dir, level+1);
            }
            var files = Directory.GetFiles(path);
            foreach (var file in files){
                FileInfo fi = new FileInfo(file);
                DisplayTabs(level+1);
                Console.WriteLine("{0} ({1}) {2}", file, fi.Length, fi.Rash());
            }
        }
    }
}