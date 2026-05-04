import React from 'react';
import { useStore } from '../store/store';
import { Product } from '../types/api';

interface ProductCardProps {
  product: Product;
}

export const ProductCard: React.FC<ProductCardProps> = ({ product }) => {
  const { cartId, addToCart, isCartLoading } = useStore();
  const [quantity, setQuantity] = React.useState(1);
  const [isAdding, setIsAdding] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);

  const handleAddToCart = async () => {
    if (!cartId) {
      setError('Cart not initialized');
      return;
    }

    if (quantity <= 0) {
      setError('Quantity must be greater than 0');
      return;
    }

    if (quantity > product.stock) {
      setError(`Only ${product.stock} items available`);
      return;
    }

    setIsAdding(true);
    setError(null);

    try {
      await addToCart(product.id, quantity);
      setQuantity(1);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to add to cart');
    } finally {
      setIsAdding(false);
    }
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-4 hover:shadow-lg transition-shadow">
      <div className="bg-gray-200 h-48 rounded mb-4 flex items-center justify-center">
        <span className="text-gray-600">📦</span>
      </div>
      
      <h3 className="text-lg font-semibold mb-2 truncate">{product.name}</h3>
      
      <p className="text-gray-600 text-sm mb-3 h-10 overflow-hidden">
        {product.description}
      </p>
      
      <div className="flex justify-between items-center mb-4">
        <span className="text-2xl font-bold text-green-600">₹{product.price.toFixed(2)}</span>
        <span className={`text-sm font-medium ${product.stock > 0 ? 'text-green-600' : 'text-red-600'}`}>
          {product.stock > 0 ? `${product.stock} in stock` : 'Out of stock'}
        </span>
      </div>

      {error && <div className="text-red-600 text-sm mb-2">{error}</div>}

      <div className="flex gap-2 mb-3">
        <input
          type="number"
          min="1"
          max={product.stock}
          value={quantity}
          onChange={(e) => setQuantity(Math.max(1, parseInt(e.target.value) || 1))}
          className="flex-1 px-2 py-2 border border-gray-300 rounded text-center"
          disabled={isAdding || isCartLoading}
        />
      </div>

      <button
        onClick={handleAddToCart}
        disabled={isAdding || isCartLoading || product.stock === 0}
        className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 disabled:bg-gray-400 transition-colors"
      >
        {isAdding ? 'Adding...' : 'Add to Cart'}
      </button>
    </div>
  );
};
