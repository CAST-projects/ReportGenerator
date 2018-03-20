/*
 *   Copyright (c) 2018 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System.IO;

namespace CastReporting.Reporting.Helper
{
    public class StreamHelper
    {
        #region METHODS
        public static decimal ByteArrayToDecimal(byte[] src)
        {
            decimal num3;
            if (src.Length < 0x10)
            {
                byte[] buffer = new byte[0x10];
                int num = 0;
                foreach (byte num2 in src)
                {
                    buffer[num++] = num2;
                }
                src = buffer;
            }
            using (MemoryStream stream = new MemoryStream(src))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    num3 = reader.ReadDecimal();
                }
            }
            return num3;
        }
        public static void CopyStream(Stream input, Stream output)
        {
            if ((input != null) && (output != null))
            {
                int num;
                byte[] buffer = new byte[0x2000];
                while ((num = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, num);
                }
            }
        }
        public static string GetString(Stream input)
        {
            using (StreamReader reader = new StreamReader(input))
            {
                return reader.ReadToEnd();
            }
        }
        #endregion METHODS
    }
}
