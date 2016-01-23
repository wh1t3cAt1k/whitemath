using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using whiteMath.General;

namespace whiteMath.Graphers
{
    [Serializable]
    public class TextGrapher: StandardGrapher
    {
        string FileName;
        int Start = 0;
        int End = 0;
        int LastRS = 0; // номер последней прочитанной строки
        Encoding enc;
        bool numex = false;

        public TextGrapher(string File_Name, Encoding FileEncoding, bool NumerationExists)
        {
            try
            {
                if (File.Exists(File_Name)) { FileName = File_Name; }
                else throw new Exception("File does not exist: " + File_Name);
                enc = FileEncoding;
                numex = NumerationExists;
            }
            catch { throw; }
        }

        public int StartingStringNumber
        {
            get { return Start; }
            set
            {
                if (value > End || value <=0 ) 
                    throw new GrapherSettingsException("Impossible to set starting string number: value <=0 or greater than ending string number.");
                else Start = value;
            }
        }

        public int EndingStringNumber
        {
            get { return End; }
            set
            {
                if (value < Start || value <= 0)
                    throw new GrapherSettingsException("Impossible to set ending string number: value <=0 or less than starting string number.");
                else End = value;
            }
        }

        public int LastReadString
        {
            get { return LastRS; }
        }

        public TextGrapher(string File_Name, Encoding FileEncoding, int StartingStringNumber, int EndingStringNumber, bool NumerationExists)
        {
            if (File.Exists(File_Name)) { FileName = File_Name; }
            else throw new GrapherSettingsException("File does not exist: " + File_Name);
            if (StartingStringNumber > EndingStringNumber || StartingStringNumber <= 0 || EndingStringNumber <= 0)
                throw new GrapherSettingsException("Wrong starting and/or ending string numbers: numeration starts from 1, ending number must be >= than starting.");
            Start = StartingStringNumber;
            End = EndingStringNumber;
            enc = FileEncoding;
            numex = NumerationExists;
        }

        public int GetPoints()
        {
            StreamReader SR = new StreamReader(new FileStream(FileName, FileMode.Open, FileAccess.Read), enc, true);
            int count = 0; // количество считанных строк
            SR.BaseStream.Seek(0, SeekOrigin.Begin);
            if (Start+End>0) while (count+1 != Start) { SR.ReadLine(); count++; }

            double x, y;
            List<Point<double>> Coll = new List<Point<double>>();
            double[] temp = new double[2];

            yMax = double.NegativeInfinity;
            yMin = double.PositiveInfinity;

            while ( Start+End>0? Start + count-1 < End: true)
            {
                if (SR.Peek()==-1) break;
                string alpha = SR.ReadLine();
                string[] sar = alpha.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                if(sar.Length!=(numex?3:2) || !double.TryParse(numex?sar[1]:sar[0], out x) ||
                    !double.TryParse(numex?sar[2]:sar[1], out y)) break;
                temp[0] = x;
                temp[1] = y;
                Coll.Add(new Point<double> (temp[0], temp[1]));
                
                if (temp[1] < yMin) yMin = temp[1];
                else if (temp[1] > yMax) yMax = temp[1];

                LastRS = Start + count+1;
                count++;
            }

            if (count == 0)
            {
                yMax = 0; 
                yMin = 0;
                throw new GrapherSettingsException("Could not read a single string from the file!");
            }

            PointsArray = Coll;
            xMax = PointsArray[PointsArray.Count-1][0];
            xMin = PointsArray[0][0];

            SR.Close();
            return count;
        }
    }

}
