using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlamourLights.Model;

namespace GlamourLights.Controller
{
    /// <summary>
    /// BLACK BOX; WILL NOT BE IMPLEMENTED!!!!!!!!!
    /// </summary>
    public class Recommender
    {
        ShopDb shopDb;

        public Recommender(ShopDb sDb)
        {
            this.shopDb = sDb;
        }
        /// <summary>
        /// BLACK BOX; WILL NOT BE IMPLEMENTED!!!!!!!!!
        /// </summary>
        /// <param name="cust"></param> customer that needs a recommendation
        /// <returns></returns> a list of recommendations
        public item[] getPersonalizedRecommendations(int cust_id)
        {
            
            var L2EQuery = shopDb.item.Where(i => i.itemId == 1);
            var recItem = L2EQuery.FirstOrDefault<item>();
            var L2EQuery2 = shopDb.item.Where(i => i.itemId == 2);
            var recItem2 = L2EQuery2.FirstOrDefault<item>();
            var L2EQuery3 = shopDb.item.Where(i => i.itemId == 3);
            var recItem3 = L2EQuery2.FirstOrDefault<item>();
            var L2EQuery4 = shopDb.item.Where(i => i.itemId == 4);
            var recItem4 = L2EQuery2.FirstOrDefault<item>();
            item[] items = new item[4];
            items[0] = recItem;
            items[1] = recItem2;
            items[2] = recItem3;
            items[3] = recItem4;
            return items;
        }



        /// <summary>
        /// BLACK BOX; WILL NOT BE IMPLEMENTED!!!!!!!!!
        /// </summary>
        /// <returns></returns> some not personalized recommendations
        public item[] getNotPersonalizedRecommendations()
        {
            var L2EQuery = shopDb.item.Where(i => i.itemId == 1);
            var recItem = L2EQuery.FirstOrDefault<item>();
            var L2EQuery2 = shopDb.item.Where(i => i.itemId == 2);
            var recItem2 = L2EQuery2.FirstOrDefault<item>();
            var L2EQuery3 = shopDb.item.Where(i => i.itemId == 3);
            var recItem3 = L2EQuery2.FirstOrDefault<item>();
            var L2EQuery4 = shopDb.item.Where(i => i.itemId == 4);
            var recItem4 = L2EQuery2.FirstOrDefault<item>();
            item[] items = new item[4];
            items[0] = recItem4;
            items[1] = recItem3;
            items[2] = recItem2;
            items[3] = recItem;
            return items;
        }
    }
}
