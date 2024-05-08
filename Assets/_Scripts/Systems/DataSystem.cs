using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSystem : SingletonPersistent<DataSystem>
{
    public User user;
    public List<Level> listOfLevel;
    public List<Achievement> listOfAchievement;
}
