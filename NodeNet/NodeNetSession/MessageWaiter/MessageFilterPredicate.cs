﻿using NodeNet.NodeNet.Message;
using NodeNet.NodeNetSession.SessionMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.MessageWaiter
{
    public delegate bool MessageFilterPredicate(SessionMessageContext context);
}
