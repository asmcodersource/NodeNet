using NodeNet.NodeNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.MessageWaiter
{
    public static class MessageFilterAndPredicate
    {
        public static MessageFilterPredicate And(MessageFilterPredicate predicateA, MessageFilterPredicate predicateB)
        {
            return (msgContext) => predicateA.Invoke(msgContext) && predicateB.Invoke(msgContext);
        }

        public static MessageFilterPredicate And(params MessageFilterPredicate[] predicates)
        {
            return (msgContext) =>
            {
                foreach (var predicate in predicates)
                    if (predicate.Invoke(msgContext) is not true)
                        return false;
                return true;
            };
        }
    }
}
