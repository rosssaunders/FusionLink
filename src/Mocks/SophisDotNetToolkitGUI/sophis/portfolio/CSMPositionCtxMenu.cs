//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.utils;

namespace sophis.portfolio
{
    public class CSMPositionCtxMenu : IDisposable
    {
        public virtual bool IsFolioAuthorized(ArrayList folioVector)
        {
            throw new NotImplementedException();
        }

        public virtual int GetContextMenuGroup()
        {
            throw new NotImplementedException();
        }

        public virtual void FolioAction(ArrayList folioVector, CMString ActionName)
        {
            throw new NotImplementedException();
        }

        public static void Register(string v, CSMPositionCtxMenu instance)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsAuthorized(ArrayList positionList)
        {
            throw new NotImplementedException();
        }

        public virtual void FolioAction(ArrayList folioVector)
        {
            throw new NotImplementedException();
        }

        public virtual void Action(ArrayList positionList, CMString ActionName)
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
        }
    }
}
