/*
 *   Copyright (c) 2019 CAST
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
using Cast.Util.Security;
using System;
using System.Xml.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// Represents a webservice connection.
    /// </summary>    
    [Serializable]
    public class WSConnection
    {
        
        /// <summary>
        /// 
        /// </summary>
        public WSConnection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// /// <param name="name"></param>
        public WSConnection(string url, string login, string password, string name)
        {
            Url = url;
            Login = login;
            Password = password;
            Name = name;
            ApiKey = false;
        }

        #region PROPERTIES

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private string _url;
        public string Url
        {
            set
            {
                _url = string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
                Uri = (!string.IsNullOrEmpty(_url)) ? new Uri(_url) : null;
            }
            get => _url;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] CryptedLogin
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the connection name
        /// </summary>    
        private string _login;
        [XmlIgnore]
        public string Login
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_login) && CryptedLogin != null)
                {
                    _login = CryptoHelper.DecryptStringFromBytes(CryptedLogin);
                }

                return _login;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    CryptedLogin = CryptoHelper.EncryptStringToBytes(value);
                }
            }
        }
        /// <summary>
        /// Get/Set the connection name
        /// </summary>       
        

        /// <summary>
        /// 
        /// </summary>
        public byte[] CryptedPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the connection name
        /// </summary>            
        private string _password;
        [XmlIgnore]
        public string Password
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_password) && CryptedPassword != null)
                {
                    _password = CryptoHelper.DecryptStringFromBytes(CryptedPassword);
                }

                return _password;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    CryptedPassword = CryptoHelper.EncryptStringToBytes(value);
                }
            }
        }

        /// <summary>
        /// Get/Set the connection name
        /// </summary>       
        public bool IsActive { get; set; }

        public bool ApiKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Uri Uri { get; private set; }


        #endregion PROPERTIES

        #region METHODS

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Uri.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var connection = obj as WSConnection;
            return (connection != null) && Uri.Equals(connection.Uri);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Uri.GetHashCode();
        }
        #endregion METHODS
    }
}
