
using UnityEngine;

class MyTransformViewPositionControl
{

    private double m_LastSerializeTime;
    private Vector3 m_NetworkPosition;
    private Vector3 m_SynchronizedSpeed;

   public Vector3 UpdatePosition(Vector3 currentPosition)
    {
        return Vector3.Lerp(currentPosition, GetExtrapolatedPosition(), Time.deltaTime * 20);
    }

    public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
    {
        m_SynchronizedSpeed = speed;
    }


    public Vector3 GetExtrapolatedPosition()
    {
        float timePassed = (float)(PhotonNetwork.time - m_LastSerializeTime);

        timePassed += (float)PhotonNetwork.GetPing() / 1000f;

        return m_NetworkPosition + m_SynchronizedSpeed * timePassed;
    }

    public void OnPhotonSerializeView(Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting == true)
        {
            SerializeData(currentPosition, stream, info);
        }
        else
        {
            DeserializeData(stream, info);

        }

        m_LastSerializeTime = info.timestamp;

    }

    void SerializeData(Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info)
    {
        stream.SendNext(currentPosition);
        m_NetworkPosition = currentPosition;
        stream.SendNext(m_SynchronizedSpeed);

    }

    void DeserializeData(PhotonStream stream, PhotonMessageInfo info)
    {
        m_NetworkPosition = (Vector3)stream.ReceiveNext();
        m_SynchronizedSpeed = (Vector3)stream.ReceiveNext();
    }
}

