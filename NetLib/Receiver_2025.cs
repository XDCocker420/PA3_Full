using Network;

public interface Receiver
    {
        void ReceiveMessage(MSG m, Transfer<MSG> t);
        void TransferDisconnected(Transfer<MSG> t);
        void AddDebugInfo(Transfer<MSG> t, String m, bool sent);
    }

