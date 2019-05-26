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
        public CSMPositionCtxMenu()
        {
        }

        public static object CreateFromFactory(string key)
        {
            throw new NotImplementedException();
        }

        protected virtual void Register(string key)
        {
        }

        public static void Register(string key, CSMPositionCtxMenu instance)
        {
            throw new NotImplementedException();
        }

        public virtual void UnRegister()
        {
        }

        public virtual void Action(ArrayList positionList)
        {
        }

        public virtual void Action(ArrayList positionList, CMString ActionName)
        {
            throw new NotImplementedException();
        }

        public virtual CSMPositionCtxMenu Clone()
        {
            throw new NotImplementedException();
        }

        public virtual void FolioAction(ArrayList folioVector)
        {
        }

        public virtual void FolioAction(ArrayList folioVector, CMString ActionName)
        {
        }

        public virtual int GetContextMenuGroup()
        {
            throw new NotImplementedException();
        }

        public CSMPositionCtxMenu GetNthElement(int nIndex)
        {
            throw new NotImplementedException();
        }

        public virtual bool HasFolioSubMenu(ArrayList folioVector)
        {
            throw new NotImplementedException();
        }

        public virtual bool HasSubMenu(ArrayList positionList)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsAuthorized(ArrayList positionList)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsFolioAuthorized(ArrayList folioVector)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
