using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Utilities
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
        //Updating Objects Errors 3 series
        public const int ObjectHasntFound = 300;
        public const int ObjectNameAlreadyUsed = 301;
        //Deleting Objects Errors 4 series

        //Installing Objects 5 series
        
    }
}
