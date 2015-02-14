using System;
using System.Collections.Generic;
using Rock.Mobile.Util.Strings;
using CCVApp.Shared.Network;
using System.Collections;

namespace CCVApp.Shared
{
    public class GroupFinder
    {
        public GroupFinder( )
        {
        }

        public class GroupEntry
        {
            public string Title { get; set; }
            public string Address { get; set; }

            public string Day { get; set; }
            public string Time { get; set; }

            public string NeighborhoodArea { get; set; }

            public double Distance { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public delegate void GetGroupsComplete( List<GroupEntry> groupEntry );
        public static void GetGroups( string street, string city, string state, string zip, GetGroupsComplete onCompletion )
        {
            List<GroupEntry> groupEntries = new List<GroupEntry>();

            // invoke the API, and when it calls our delegate, we can then invoke our original caller's onCompletion
            RockApi.Instance.GetGroupsByLocation( CCVApp.Shared.Config.GeneralConfig.NeighborhoodGroupGeoFenceValueId, 
                CCVApp.Shared.Config.GeneralConfig.NeighborhoodGroupValueId,
                street, city, state, zip,
                delegate(System.Net.HttpStatusCode statusCode, string statusDescription, List<Rock.Client.Group> model )
                {
                    if ( Rock.Mobile.Network.Util.StatusInSuccessRange( statusCode ) == true )
                    {
                        Random rand = new Random( );


                        // first thing we receive is the "area" group(s)
                        foreach ( Rock.Client.Group areaGroup in model )
                        {
                            // in each area, there's an actual small group
                            foreach ( Rock.Client.Group smallGroup in areaGroup.Groups )
                            {
                                // get the group location out of the small group enumerator
                                IEnumerator enumerator = smallGroup.GroupLocations.GetEnumerator( );
                                enumerator.MoveNext( );
                                Rock.Client.Location location = ( (Rock.Client.GroupLocation)enumerator.Current ).Location;

                                // and of course, each group has a location
                                GroupEntry entry = new GroupEntry();
                                entry.Title = smallGroup.Name;
                                entry.Address = location.Street1 + "\n" + location.City + ", " + location.State + " " + location.PostalCode.Substring( 0, Math.Max( 0, location.PostalCode.IndexOf( '-' ) ) );
                                entry.NeighborhoodArea = areaGroup.Name;

                                //todo: Get the real info for these
                                entry.Distance = rand.NextDouble( ) * 10.0f;
                                entry.Day = "Thursday";
                                entry.Time = "7pm";
                                //

                                entry.Latitude = location.Latitude.Value;
                                entry.Longitude = location.Longitude.Value;

                                groupEntries.Add( entry );
                            }
                        }
                    }

                    // our network delegate has been invoked and compelted, so now call whoever called us.
                    onCompletion( groupEntries );
                } );
        }
    }
}

