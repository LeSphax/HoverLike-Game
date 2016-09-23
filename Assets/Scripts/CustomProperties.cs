using UnityEngine;
using UnityEngine.Assertions;

public class CustomProperties
{

    public static T GetProperty<T>(string propertyName)
    {
        object property;
        if (PhotonNetwork.room.customProperties != null)
            if (PhotonNetwork.room.customProperties.TryGetValue(propertyName, out property))
            {
                Assert.IsTrue(typeof(T).IsAssignableFrom(property.GetType()));
                return (T)property;
            }
        return default(T);
    }
}
