using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceShoot
{
    class EntityQueue
    {
        Queue<Entity> queue { get; set; }

        public EntityQueue(Queue<Entity> queue)
        {
            this.queue = queue;
        }
    }
}
