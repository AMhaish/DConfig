using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.Utilities
{

    /// <summary>
    /// 
    /// </summary>
    public static class ResultCodes
    {
        public const int UnknownError = 0;
        public const int Succeed = 1;

        
        //Creating Objects Errors 2 series
        public const int ObjectAlreadyExists = 200;
        public const int ObjectSavedWithAnotherName = 201;
        public const int MissingInfo = 202;
        //Updating Objects Errors 3 series
        public const int ObjectHasntFound = 300;
        public const int ObjectNameAlreadyUsed = 301;
        public const int ObjectNotAllowedToBeUpdated = 302;
        public const int ObjectEmpty = 303;
        public const int ObjectResourceHasntFound = 304;
        public const int ObjectResourceUnknwonError = 305;
        public const int ObjectAlreadyUpdated = 306;
        public const int ObjectInvalid = 307;
        //Deleting Objects Errors 4 series
        public const int ObjectLinkedToAnotherObject = 400;
        public const int ObjectNotAllowedToBeDeleted = 402;
        //Installing Objects 5 series
        
    }
}
