using System;
using CCVApp.Shared.Config;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using Rock.Mobile.Network;
using RestSharp;
using System.Net;

namespace CCVApp.Shared
{
    /// <summary>
    /// A class that allows files to be written to disk (cached), and loaded later.
    /// The typical use is to download a file and write it to cache. On load,
    /// ask the cache for it, and if null is returned,  download it.
    /// </summary>
    public class FileCache
    {
        static FileCache _Instance = new FileCache( );
        public static FileCache Instance { get { return _Instance; } }

        System.Collections.Hashtable CacheMap { get; set; }

        object locker = new object();

        public FileCache( )
        {
            // ensure the cache directory exists
            if ( Directory.Exists( CachePath ) == false )
            {
                Directory.CreateDirectory( CachePath );
            }

            LoadCacheMap( );
        }

        public void LoadCacheMap( )
        {
            // attemp to read the cache map
            try
            {
                using ( FileStream reader = new FileStream( CachePath + "/" + "cache.dat", FileMode.Open ) )
                {
                    StreamReader mapStream = new StreamReader( reader );
                    BinaryFormatter formatter = new BinaryFormatter( );

                    CacheMap = (System.Collections.Hashtable)formatter.Deserialize( mapStream.BaseStream );

                    CleanUp( );
                }
            }
            catch( Exception )
            {
                // if it fails for any reason, simply create a new one
                CacheMap = new System.Collections.Hashtable();
            }
        }

        public void SaveCacheMap( )
        {
            try
            {
                using( FileStream writer = new FileStream( CachePath + "/" + "cache.dat", FileMode.Create ) )
                {
                    BinaryFormatter formatter = new BinaryFormatter( );
                    StreamWriter mapStream = new StreamWriter( writer );
                    formatter.Serialize( mapStream.BaseStream, CacheMap );
                }
            }
            catch( Exception )
            {
            }
        }

        string CachePath
        {
            get
            {
                // get the path based on the platform
                #if __IOS__
                //Note: Xamarin warns that we should use the below commented out version. But it doesn't work...the original does. 
                //http://developer.xamarin.com/guides/ios/application_fundamentals/working_with_the_file_system/ Their comment here says it's temporary, so maybe the docs are out of date?
                //string cachePath = MonoTouch.Foundation.NSFileManager.DefaultManager.GetUrls (MonoTouch.Foundation.NSSearchPathDirectory.DocumentDirectory, MonoTouch.Foundation.NSSearchPathDomain.User) [0].ToString();
                string cachePath = System.IO.Path.Combine ( Environment.GetFolderPath(Environment.SpecialFolder.Personal), "" );
                #else
                string cachePath = Rock.Mobile.PlatformSpecific.Android.Core.Context.GetExternalFilesDir( null ).ToString( );
                #endif

                cachePath += "/" + GeneralConfig.CacheDirectory;
                return cachePath;
            }
        }

        /// <summary>
        /// Scans the cache hashtable and removes any entries that are expired. Additionally, it deletes the file
        /// from the cache folder.
        /// </summary>
        public void CleanUp( )
        {
            lock ( locker )
            {
                Console.WriteLine( "Running cleanup" );

                List< DictionaryEntry > expiredItems = new List< DictionaryEntry >( );

                // scan our cache and remove anything older than the expiration time.
                foreach ( DictionaryEntry entry in CacheMap )
                {
                    DateTime entryValue = (DateTime) entry.Value;

                    // if it's older than our expiration time, delete it
                    TimeSpan deltaTime = ( DateTime.Now - entryValue );
                    if ( DateTime.Now >= entryValue )
                    {
                        // delete the entry
                        File.Delete( CachePath + "/" + entry.Key );

                        expiredItems.Add( entry );

                        Console.WriteLine( "{0} expired. Age: {1} minutes old.", (string)entry.Key, deltaTime.Minutes );
                    }
                    else
                    {
                        Console.WriteLine( "{0} still fresh NOT REMOVING.", (string)entry.Key );
                    }
                }

                // remove all entries that have been expired
                foreach ( DictionaryEntry entry in expiredItems )
                {
                    CacheMap.Remove( entry.Key );
                }

                Console.WriteLine( "Cleanup complete" );
            }
        }

        public bool SaveFile( object buffer, string filename, TimeSpan? expirationTime = null )
        {
            // sync point so we don't read multiple times at once.
            // Note: If we ever need to support multiple threads reading from the cache at once, we'll need
            // a table to hash multiple locks per filename
            lock ( locker )
            {
                bool result = false;

                try
                {
                    // attempt to write the file out to disk
                    using ( FileStream writer = new FileStream( CachePath + "/" + filename, FileMode.Create ) )
                    {
                        BinaryFormatter formatter = new BinaryFormatter( );
                        StreamWriter mapStream = new StreamWriter( writer );
                        formatter.Serialize( mapStream.BaseStream, buffer );

                        mapStream.Dispose( );

                        // store in our cachemap the filename and when we wrote it.
                        // Don't ever add it twice. If it exists, remove it
                        if( CacheMap.Contains( filename ) )
                        {
                            CacheMap.Remove( filename );
                            Console.WriteLine( "{0} is already cached. Updating time to {1}", filename, DateTime.Now );
                        }
                        else
                        {
                            Console.WriteLine( "Adding {0} to cache. Time {1}", filename, DateTime.Now );
                        }

                        if( expirationTime.HasValue == false )
                        {
                            expirationTime = GeneralConfig.CacheFileDefaultExpiration;
                        }

                        // and now it'll be added a second time.
                        CacheMap.Add( filename, DateTime.Now + expirationTime.Value );

                        // if an exception occurs we won't set result to true
                        result = true;
                    }
                }
                catch ( Exception )
                {

                }

                // return the result
                return result;
            }
        }

        public bool FileExists( string filename )
        {
            return File.Exists( CachePath + "/" + filename );
        }

        public object LoadFile( string filename )
        {
            try
            {
                using ( FileStream reader = new FileStream( CachePath + "/" + filename, FileMode.Open ) )
                {
                    StreamReader mapStream = new StreamReader( reader );
                    BinaryFormatter formatter = new BinaryFormatter( );

                    object loadedObj = formatter.Deserialize( mapStream.BaseStream );
                    mapStream.Dispose( );

                    return loadedObj;
                }
            }
            catch( Exception )
            {
            }

            return null;
        }

        public void RemoveFile( string filename )
        {
            // delete the entry
            File.Delete( CachePath + "/" + filename );
            CacheMap.Remove( filename );
        }

        public delegate void ImageDownloaded( string imageUrl, string cachedFilename, bool result );
        public void DownloadFileToCache( string downloadUrl, string cachedFilename, ImageDownloaded callback )
        {
            if ( string.IsNullOrEmpty( downloadUrl ) == false )
            {
                // request the image for the series
                HttpRequest webRequest = new HttpRequest();
                RestRequest restRequest = new RestRequest( Method.GET );
                restRequest.AddHeader( "Accept", "image/png, text/*" );

                webRequest.ExecuteAsync( downloadUrl, restRequest, 
                    delegate(HttpStatusCode statusCode, string statusDescription, byte[] model )
                    {
                        bool result = false;

                        if ( Rock.Mobile.Network.Util.StatusInSuccessRange( statusCode ) == true )
                        {
                            // write it to cache
                            MemoryStream imageBuffer = new MemoryStream( model );
                            imageBuffer.Position = 0;
                            FileCache.Instance.SaveFile( imageBuffer, cachedFilename );
                            imageBuffer.Dispose( );

                            result = true;
                        }

                        if( callback != null )
                        {
                            callback( downloadUrl, cachedFilename, result );
                        }
                    } );
            }
        }
    }
}

