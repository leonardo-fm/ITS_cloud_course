# Progetto per corso di cloud

Setup MongoDB (https://docs.mongodb.com/manual/reference/command/)

	- Create DB
	- Create 2 Collections, the first for the photos and second one for the useres
	- Create a dedicate user for the dib with credentals

Setup Project

	- Fill all the fields and put all the code in a .cs file in the CloudSite/CloudSite/Models named Variables.cs

#############################################################

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudSite.Models
{
    public static class Variables
    {
        public const string HOST_FOR_MONGODB = "";
        public const string PORT_FOR_MONGODB = "";
        public const string USER_FOR_AUTHENTICATION_MONGODB = "";
        public const string PASSWORD_FOR_AUTHENTICATION_MONGODB = "";
        public const string NAME_OF_DATABASE_IN_MONGODB = "";
        public const string NAME_OF_TABLE_FOR_USERS_IN_MONGODB = "";
        public const string NAME_OF_TABLE_FOR_PHOTOS_IN_MONGODB = "";

        public const string EMAIL_ADDRESS_FOR_SENDING_EMAILS = "";
        public const string PASSWORD_FOR_EMAIL_ADDRESS = "";

        public const string SUBSCRIPTION_KEY_FOR_AI_VISION = "";
        public const string ENDPOINT_FOR_AI_VISION = "";

        public const string ACCOUNT_NAME_FOR_BLOB_STORAGE = "";
        public const string SUBSCRIPTION_KEY_FOR_BLOB_STORAGE = "";
        public const string ENDPOINT_SUFFIX_FOR_BLOB_STORAGE = "";
        
	//The file path contains also the name of the log file (es @"C:\Users\Jhon\Desktop\Log\Log.log")
        public const string LOG_FILE_PATH = @"";
    }
}

#############################################################