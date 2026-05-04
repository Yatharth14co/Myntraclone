import { create } from 'zustand';
import { devtools } from 'zustand/middleware';
import { Cart, Product, OrderConfirmation } from '../types/api';
import { apiService } from '../services/api';

interface StoreState {
  // Cart state
  cartId: number | null;
  cart: Cart | null;
  isCartLoading: boolean;
  cartError: string | null;

  // Products state
  products: Product[];
  isProductsLoading: boolean;
  productsError: string | null;
  currentPage: number;
  pageSize: number;
  totalPages: number;

  // Order state
  order: OrderConfirmation | null;
  isCheckingOut: boolean;
  checkoutError: string | null;

  // UI state
  searchTerm: string;

  // Actions
  initCart: (cartId: number) => void;
  fetchCart: () => Promise<void>;
  addToCart: (productId: number, quantity: number) => Promise<void>;
  applyCoupon: (couponCode: string) => Promise<void>;
  clearCartAction: () => Promise<void>;

  fetchProducts: (page?: number, search?: string) => Promise<void>;
  setSearchTerm: (term: string) => void;

  checkout: () => Promise<void>;
  clearOrderState: () => void;

  clearErrors: () => void;
}

export const useStore = create<StoreState>()(
  devtools((set, get) => ({
    // Initial state
    cartId: null,
    cart: null,
    isCartLoading: false,
    cartError: null,

    products: [],
    isProductsLoading: false,
    productsError: null,
    currentPage: 1,
    pageSize: 10,
    totalPages: 1,

    order: null,
    isCheckingOut: false,
    checkoutError: null,

    searchTerm: '',

    // Actions
    initCart: (cartId: number) => {
      set({ cartId });
    },

    fetchCart: async () => {
      const { cartId } = get();
      if (!cartId) return;

      set({ isCartLoading: true, cartError: null });
      try {
        const cart = await apiService.getCart(cartId);
        set({ cart });
      } catch (error) {
        set({ cartError: error instanceof Error ? error.message : 'Failed to fetch cart' });
      } finally {
        set({ isCartLoading: false });
      }
    },

    addToCart: async (productId: number, quantity: number) => {
      const { cartId } = get();
      if (!cartId) throw new Error('Cart not initialized');

      set({ isCartLoading: true, cartError: null });
      try {
        const cart = await apiService.addToCart(cartId, productId, quantity);
        set({ cart });
      } catch (error) {
        set({ cartError: error instanceof Error ? error.message : 'Failed to add to cart' });
        throw error;
      } finally {
        set({ isCartLoading: false });
      }
    },

    applyCoupon: async (couponCode: string) => {
      const { cartId } = get();
      if (!cartId) throw new Error('Cart not initialized');

      set({ isCartLoading: true, cartError: null });
      try {
        const cart = await apiService.applyCoupon(cartId, couponCode);
        set({ cart });
      } catch (error) {
        set({ cartError: error instanceof Error ? error.message : 'Failed to apply coupon' });
        throw error;
      } finally {
        set({ isCartLoading: false });
      }
    },

    clearCartAction: async () => {
      const { cartId } = get();
      if (!cartId) return;

      set({ isCartLoading: true, cartError: null });
      try {
        await apiService.clearCart(cartId);
        set({ cart: null });
      } catch (error) {
        set({ cartError: error instanceof Error ? error.message : 'Failed to clear cart' });
      } finally {
        set({ isCartLoading: false });
      }
    },

    fetchProducts: async (page = 1, search = '') => {
      set({ isProductsLoading: true, productsError: null, currentPage: page, searchTerm: search });
      try {
        const response = await apiService.getProducts(page, 10, search);
        set({
          products: response.items,
          totalPages: response.totalPages,
          currentPage: response.pageNumber,
        });
      } catch (error) {
        set({ productsError: error instanceof Error ? error.message : 'Failed to fetch products' });
      } finally {
        set({ isProductsLoading: false });
      }
    },

    setSearchTerm: (term: string) => {
      set({ searchTerm: term });
    },

    checkout: async () => {
      const { cartId } = get();
      if (!cartId) throw new Error('Cart not initialized');

      set({ isCheckingOut: true, checkoutError: null });
      try {
        const order = await apiService.checkout(cartId);
        set({ order });
      } catch (error) {
        set({ checkoutError: error instanceof Error ? error.message : 'Checkout failed' });
        throw error;
      } finally {
        set({ isCheckingOut: false });
      }
    },

    clearOrderState: () => {
      set({ order: null });
    },

    clearErrors: () => {
      set({ cartError: null, productsError: null, checkoutError: null });
    },
  }))
);
