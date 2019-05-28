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
using System;
using CastReporting.Domain;
using CastReporting.Mediation.Interfaces;
using CastReporting.Repositories;
using CastReporting.Repositories.Interfaces;

namespace CastReporting.BLL
{
    public abstract class BaseBLL : IDisposable
    {
        // <summary>
        //
        // </summary>
        protected WSConnection Connection { get; set; }

        protected static ICastProxy Client;

        /// <summary>
        /// 
        /// </summary>
        protected ICastRepsitory GetRepository()
        {
            CastRepository repo = new CastRepository(Connection, Client);
            Client = repo.GetClient();
            return repo;
        }

        protected static ICastRepsitory GetRepository(WSConnection connection, bool dropCookie=false)
        {
            CastRepository repo = dropCookie ? new CastRepository(connection, null) : new CastRepository(connection, Client);
            Client = repo.GetClient();
            return repo;
        }


        /// <summary>
        /// 
        /// </summary>
        protected BaseBLL()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        protected BaseBLL(WSConnection connection)
        {
            Connection = connection;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
