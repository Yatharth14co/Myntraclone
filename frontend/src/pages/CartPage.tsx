import React, { useEffect } from 'react';
import { useStore } from '../store/store';
import { CouponForm } from '../components/CouponForm';

export const CartPage: React.FC = () => {
  const { 
    cartId, 
    cart, 
    isCartLoading, 
    cartError,
    fetchCart,
    checkout,
    isCheckingOut,
    order
  } = useStore();

  useEffect(() => {
    if (cartId) {
      fetchCart();
    }
  }, [cartId]);

  if (!cartId) {
    return (
      <div className="min-h-screen bg-gray-100 p-4 flex items-center justify-center">
        <div className="text-center">
          <h1 className="text-2xl font-bold mb-4">Cart Error</h1>
          <p className="text-gray-600">Cart not initialized. Please refresh the page.</p>
        </div>
      </div>
    );
  }

  if (isCartLoading && !cart) {
    return (
      <div className="min-h-screen bg-gray-100 p-4 flex items-center justify-center">
        <div className="text-center">Loading cart...</div>
      </div>
    );
  }

  if (!cart || cart.items.length === 0) {
    return (
      <div className="min-h-screen bg-gray-100 p-4">
        <div className="max-w-4xl mx-auto">
          <h1 className="text-3xl font-bold mb-6">Shopping Cart</h1>
          <div className="bg-white rounded-lg shadow p-8 text-center">
            <p className="text-gray-600 mb-4">Your cart is empty</p>
            <a href="/" className="text-blue-600 hover:underline">
              Continue shopping
            </a>
          </div>
        </div>
      </div>
    );
  }

  const handleCheckout = async () => {
    try {
      await checkout();
    } catch (error) {
      // Error is handled in the store
    }
  };

  if (order) {
    return (
      <div className="min-h-screen bg-gray-100 p-4">
        <div className="max-w-2xl mx-auto">
          <div className="bg-green-50 border border-green-200 rounded-lg p-8 text-center">
            <h2 className="text-3xl font-bold text-green-800 mb-4">✓ Order Placed!</h2>
            <p className="text-gray-700 mb-4">Thank you for your purchase</p>
            
            <div className="bg-white rounded-lg p-4 mb-6 text-left">
              <p className="text-sm text-gray-600 mb-2">Order ID: <strong>#{order.orderId}</strong></p>
              <p className="text-sm text-gray-600 mb-2">
                Total Amount: <strong className="text-lg text-green-600">₹{order.totalAmount.toFixed(2)}</strong>
              </p>
              <p className="text-sm text-gray-600">
                Ordered at: <strong>{new Date(order.orderedAt).toLocaleString()}</strong>
              </p>
            </div>

            <a href="/" className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700">
              Continue Shopping
            </a>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-100 p-4">
      <div className="max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold mb-6">Shopping Cart</h1>

        {cartError && (
          <div className="bg-red-100 text-red-800 p-4 rounded mb-4">
            {cartError}
          </div>
        )}

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Cart Items */}
          <div className="lg:col-span-2">
            <div className="bg-white rounded-lg shadow overflow-hidden">
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="bg-gray-50 border-b">
                    <tr>
                      <th className="px-4 py-3 text-left text-sm font-semibold">Product</th>
                      <th className="px-4 py-3 text-center text-sm font-semibold">Price</th>
                      <th className="px-4 py-3 text-center text-sm font-semibold">Qty</th>
                      <th className="px-4 py-3 text-right text-sm font-semibold">Total</th>
                    </tr>
                  </thead>
                  <tbody>
                    {cart.items.map((item) => (
                      <tr key={item.id} className="border-b hover:bg-gray-50">
                        <td className="px-4 py-3 text-sm">{item.productName}</td>
                        <td className="px-4 py-3 text-center text-sm">₹{item.unitPrice.toFixed(2)}</td>
                        <td className="px-4 py-3 text-center text-sm">{item.quantity}</td>
                        <td className="px-4 py-3 text-right text-sm font-semibold">
                          ₹{item.lineTotal.toFixed(2)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>

            {/* Coupon Form */}
            <div className="mt-6">
              <CouponForm onApplied={() => fetchCart()} />
            </div>
          </div>

          {/* Order Summary */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow p-6 sticky top-4">
              <h2 className="text-lg font-bold mb-4">Order Summary</h2>

              <div className="space-y-3 mb-4">
                <div className="flex justify-between text-sm">
                  <span className="text-gray-600">Subtotal:</span>
                  <span className="font-medium">₹{cart.subtotal.toFixed(2)}</span>
                </div>
                {cart.discount > 0 && (
                  <div className="flex justify-between text-sm text-green-600">
                    <span>Discount ({cart.couponCode}):</span>
                    <span className="font-medium">-₹{cart.discount.toFixed(2)}</span>
                  </div>
                )}
                <div className="flex justify-between text-sm">
                  <span className="text-gray-600">Tax (18%):</span>
                  <span className="font-medium">
                    ₹{((cart.subtotal - cart.discount) * 0.18).toFixed(2)}
                  </span>
                </div>
              </div>

              <div className="border-t pt-4 mb-6">
                <div className="flex justify-between text-lg font-bold">
                  <span>Total:</span>
                  <span className="text-green-600">
                    ₹{(cart.total + (cart.subtotal - cart.discount) * 0.18).toFixed(2)}
                  </span>
                </div>
              </div>

              <button
                onClick={handleCheckout}
                disabled={isCheckingOut || isCartLoading}
                className="w-full bg-green-600 text-white py-3 rounded-lg hover:bg-green-700 disabled:bg-gray-400 font-semibold transition-colors"
              >
                {isCheckingOut ? 'Processing...' : 'Proceed to Checkout'}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
