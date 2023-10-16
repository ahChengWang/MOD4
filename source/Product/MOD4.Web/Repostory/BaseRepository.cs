﻿using Utility.Helper;

namespace MOD4.Web.Repostory
{
    public class BaseRepository
    {
        protected MSSqlDBHelper _dbHelper;
        protected OracleDBHelper _oracleDBHelper;

        //protected LoggerWrapper _loggerWrapper;

        //public BaseRepository(LoggerWrapper loggerWrapper)
        //{
        //    _dbHelper = new MSSqlDBHelper();
        //    _loggerWrapper = loggerWrapper;
        //}


        public BaseRepository()
        {
            _dbHelper = new MSSqlDBHelper();
            _oracleDBHelper = new OracleDBHelper();
        }
    }
}
