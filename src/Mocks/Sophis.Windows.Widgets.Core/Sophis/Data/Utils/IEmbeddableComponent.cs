//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

namespace Sophis.Data.Utils
{
    public interface IEmbeddableComponent
    {
        void OnActivated();

        void OnDeactivated();

        void OnLoaded();

        void OnUnloaded();
    }
}