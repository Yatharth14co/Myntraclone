import React from 'react';
import { useStore } from '../store/store';

interface CouponFormProps {
  onApplied?: () => void;
}

export const CouponForm: React.FC<CouponFormProps> = ({ onApplied }) => {
  const { applyCoupon, isCartLoading, cart } = useStore();
  const [couponCode, setCouponCode] = React.useState('');
  const [error, setError] = React.useState<string | null>(null);
  const [success, setSuccess] = React.useState<string | null>(null);

  const handleApplyCoupon = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!couponCode.trim()) {
      setError('Please enter a coupon code');
      return;
    }

    setError(null);
    setSuccess(null);

    try {
      await applyCoupon(couponCode.toUpperCase());
      setSuccess('Coupon applied successfully!');
      setCouponCode('');
      onApplied?.();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to apply coupon');
    }
  };

  return (
    <form onSubmit={handleApplyCoupon} className="bg-gray-50 p-4 rounded-lg">
      <h3 className="text-lg font-semibold mb-3">Apply Coupon</h3>
      
      {cart?.couponCode && (
        <div className="bg-green-100 text-green-800 p-2 rounded mb-3">
          Active coupon: <strong>{cart.couponCode}</strong>
        </div>
      )}
      
      <div className="flex gap-2">
        <input
          type="text"
          placeholder="Enter coupon code"
          value={couponCode}
          onChange={(e) => setCouponCode(e.target.value)}
          disabled={isCartLoading}
          className="flex-1 px-3 py-2 border border-gray-300 rounded focus:outline-none focus:border-blue-500"
        />
        <button
          type="submit"
          disabled={isCartLoading}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:bg-gray-400 transition-colors"
        >
          Apply
        </button>
      </div>

      {error && <div className="text-red-600 text-sm mt-2">{error}</div>}
      {success && <div className="text-green-600 text-sm mt-2">{success}</div>}
      
      <p className="text-xs text-gray-600 mt-2">
        Try codes: <strong>FLAT50</strong>, <strong>SAVE10</strong>, <strong>WELCOME20</strong>
      </p>
    </form>
  );
};
