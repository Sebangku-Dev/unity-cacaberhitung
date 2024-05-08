using System.Collections.Generic;
using UnityEngine;

public class BaseSystem : SingletonPersistent<BaseSystem>
{
    public User userData;
    public List<Level> listOfLevel;
    public List<Achievement> listOfAchievement;
}
