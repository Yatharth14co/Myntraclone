import React from 'react';
import { ProductsPage } from './pages/ProductsPage';
import { CartPage } from './pages/CartPage';
import { useStore } from './store/store';

type Page = 'products' | 'cart';

export const App: React.FC = () => {
  const [currentPage, setCurrentPage] = React.useState<Page>('products');
  const { cartId, initCart } = useStore();

  React.useEffect(() => {
    // Initialize cart ID (in a real app, this might come from a backend or auth)
    if (!cartId) {
      initCart(1); // Use a fixed cart ID for demo
    }
  }, [cartId, initCart]);

  return (
    <div>
      {/* Navigation */}
      <nav className="bg-blue-600 text-white shadow-lg">
        <div className="max-w-6xl mx-auto px-4 py-4 flex justify-between items-center">
          <h1 className="text-2xl font-bold">E-Commerce Store</h1>
          <div className="flex gap-4">
            <button
              onClick={() => setCurrentPage('products')}
              className={`px-4 py-2 rounded ${currentPage === 'products' ? 'bg-blue-800' : 'hover:bg-blue-700'}`}
            >
              Shop
            </button>
            <button
              onClick={() => setCurrentPage('cart')}
              className={`px-4 py-2 rounded ${currentPage === 'cart' ? 'bg-blue-800' : 'hover:bg-blue-700'}`}
            >
              🛒 Cart
            </button>
          </div>
        </div>
      </nav>

      {/* Main Content */}
      {currentPage === 'products' && <ProductsPage />}
      {currentPage === 'cart' && <CartPage />}
    </div>
  );
};
