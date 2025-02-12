using UnityEngine;

namespace ColliderHandler
{
    public abstract class ColliderHandler<T> : MonoBehaviour
    {
        protected T _tComponent;
        protected float _damage;
        protected int _index;

        public virtual void InitializeHandler(T tComponent, float damage, int index)
        {
            _tComponent = tComponent;
            _damage = damage;
            _index = index;
        }

        protected abstract void IsColliding(Collider other, int index);
    }
}
