using IOBootstrap.NET.Core.Database;
using Realms;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace IOBootstrap.NET.Common.Entities.AutoIncrements
{
    public class IOAutoIncrementsEntity : RealmObject
    {

        #region Properties

        [PrimaryKey]
        public string className { get; set; }
        public int autoId { get; set; }

        #endregion


        #region Helper Methods

        public static int IdForClass(IIODatabase database, Type classType) {
            // Obtain realm 
            Realm realm = database.GetRealmForMainThread();

            // Obtain entity for auto increments
            var autoIncrementsEntity = realm.Find<IOAutoIncrementsEntity>(classType.Name);

            // Create id value property
            int idValue = 0;

            // Check auto increment entity is not null
            if (autoIncrementsEntity != null) {
                // Then return id
                idValue = autoIncrementsEntity.autoId;
            }

            // Increase id value
            idValue += 1;

            // Set id for class
            IOAutoIncrementsEntity.SetIdForClass(database, idValue, classType.Name);

            // Then return id value
            return idValue;
        }

        private static void SetIdForClass(IIODatabase database, int autoId, string className) {
            // Create auto increments entity
            IOAutoIncrementsEntity autoIncrementsEntity = new IOAutoIncrementsEntity()
            {

                // Update entity properties
                autoId = autoId,
                className = className
            };

            // Listen updates entity
            database.UpdateEntity(autoIncrementsEntity)
                    .Subscribe();
        }

        #endregion
    }
}
