# [Final project for the cloud course](https://github.com/GlobalBlackout/ProgettoCloud)

## Requirements

- [MongoDB](https://docs.mongodb.com)
- [IIS](https://www.microsoft.com/en-us/download/details.aspx?id=48264)
- [Gmail Account](https://accounts.google.com/signup/v2/webcreateaccount?flowName=GlifWebSignIn&flowEntry=SignUp) for sending emails
- [Azure account](https://azure.microsoft.com/en-us/)
- [Azure Computer Vision](https://azure.microsoft.com/en-us/services/cognitive-services/computer-vision/)
- [Azure Blob Storage](https://azure.microsoft.com/en-us/services/storage/blobs/)

## Setup [MongoDB](https://docs.mongodb.com)

- Create DB
- Create 2 Collections, the first for the photos and second one for the useres
- Create a dedicate user for the db with credentals

```mongo
use DATABASE_NAME

db.createCollection("COLLECTION_FOR_USERS")

db.createCollection("COLLECTION_FOR_PHOTOS")

db.createUser(
   {
     user: "<USERNAME_FOR_DB>",
     pwd: passwordPrompt(),  // Or  "<PASSWORD_FOR_DB>"
     roles: [ { role: "readWrite", db: "<DATABASE_NAME>" } ]
   }
)
```

## Setup Project

- Fill **all** the fields and put all the code in a .cs file in the **CloudSite/CloudSite/Models** named **Variables.cs**

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

```C#
namespace CloudSite.Models
{
    public static class Variables
    {
        public const string HOST_FOR_MONGODB = "";
        public const string PORT_FOR_MONGODB = "";
        public const string USER_FOR_AUTHENTICATION_MONGODB = "";
        public const string PASSWORD_FOR_AUTHENTICATION_MONGODB = "";
	public const string ADDITIONAL_PARAMETERS_FOR_CONNECTION = @"";

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
        
	// The file path contains also the name of the log file (es @"C:\Users\Jhon\Desktop\Log\Log.log")
        public const string LOG_FILE_PATH = @"";
	// Like @"https://localhost:44324"
	public const string URL_OF_THE_HOST = @"";
    }
}
```

## Roslyn.zip
For some reason the project can't create roslyn's folder inside ProgettoCloud\CloudSite\bin, so I attach a folder zipped to extract and put in the folder ProgettoCloud\CloudSite\bin

### Copyright

The authors of this software are 
[Bert Lorenzo](https://github.com/LorenzoBert),
[Carbonati Davide](https://github.com/DaviCarbo) and 
[Ferrero-Merlino Leonardo](https://github.com/GlobalBlackout/).

This software is released under the [Apache License](/LICENSE), Version 2.0.
