export interface Product {
  id: number;
  name: string;
  price: number;
  stock: number;
  description: string;
}

export interface CartItem {
  id: number;
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface Cart {
  id: number;
  items: CartItem[];
  couponCode: string | null;
  subtotal: number;
  discount: number;
  total: number;
}

export interface OrderItem {
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface OrderConfirmation {
  orderId: number;
  items: OrderItem[];
  subtotal: number;
  discount: number;
  tax: number;
  totalAmount: number;
  couponCode: string | null;
  status: string;
  orderedAt: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors?: Record<string, string[]>;
}

export interface PaginatedProducts {
  items: Product[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}
