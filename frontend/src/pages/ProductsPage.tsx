import React, { useEffect } from 'react';
import { useStore } from '../store/store';
import { ProductCard } from '../components/ProductCard';

export const ProductsPage: React.FC = () => {
  const { 
    products, 
    isProductsLoading, 
    productsError, 
    currentPage, 
    totalPages,
    searchTerm,
    setSearchTerm,
    fetchProducts 
  } = useStore();

  useEffect(() => {
    fetchProducts(1, searchTerm);
  }, []);

  const handleSearch = (term: string) => {
    setSearchTerm(term);
    fetchProducts(1, term);
  };

  const handlePrevious = () => {
    if (currentPage > 1) {
      fetchProducts(currentPage - 1, searchTerm);
    }
  };

  const handleNext = () => {
    if (currentPage < totalPages) {
      fetchProducts(currentPage + 1, searchTerm);
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 p-4">
      <div className="max-w-6xl mx-auto">
        <h1 className="text-4xl font-bold mb-6">Shop Products</h1>

        {/* Search Bar */}
        <div className="mb-6">
          <input
            type="text"
            placeholder="Search products..."
            value={searchTerm}
            onChange={(e) => handleSearch(e.target.value)}
            className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:outline-none focus:border-blue-500"
          />
        </div>

        {/* Error Message */}
        {productsError && (
          <div className="bg-red-100 text-red-800 p-4 rounded mb-4">
            {productsError}
          </div>
        )}

        {/* Loading State */}
        {isProductsLoading && (
          <div className="text-center py-8">
            <div className="inline-block">Loading products...</div>
          </div>
        )}

        {/* Products Grid */}
        {!isProductsLoading && products.length > 0 && (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-6">
              {products.map((product) => (
                <ProductCard key={product.id} product={product} />
              ))}
            </div>

            {/* Pagination */}
            <div className="flex justify-between items-center">
              <button
                onClick={handlePrevious}
                disabled={currentPage === 1 || isProductsLoading}
                className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:bg-gray-400"
              >
                Previous
              </button>
              <span className="text-gray-700">
                Page {currentPage} of {totalPages}
              </span>
              <button
                onClick={handleNext}
                disabled={currentPage === totalPages || isProductsLoading}
                className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:bg-gray-400"
              >
                Next
              </button>
            </div>
          </>
        )}

        {/* Empty State */}
        {!isProductsLoading && products.length === 0 && !productsError && (
          <div className="text-center py-8 text-gray-600">
            No products found
          </div>
        )}
      </div>
    </div>
  );
};
