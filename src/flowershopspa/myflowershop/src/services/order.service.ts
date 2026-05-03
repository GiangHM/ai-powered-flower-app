import httpService from '@/services/http.services'
import type { CreateOrderDto, OrderResponseDto, UpdateOrderStatusDto } from '@/models/order'

class OrderService {
  private readonly BASE = '/api/Orders'
  private readonly ADMIN_BASE = '/api/FlowerManagement/Orders'

  /**
   * Place a new order (anonymous or authenticated)
   * @param dto - Order payload with customer ID and items
   * @returns Promise with the created order response
   */
  async placeOrder(dto: CreateOrderDto): Promise<OrderResponseDto> {
    const res = await httpService.post<OrderResponseDto>(this.BASE, dto)
    return (res as any).data ?? res
  }

  /**
   * Get an order by ID (requires authentication via JWT interceptor)
   * @param id - Order ID
   * @returns Promise with order details
   */
  async getOrderById(id: number): Promise<OrderResponseDto> {
    const res = await httpService.get<OrderResponseDto>(`${this.BASE}/${id}`)
    return (res as any).data ?? res
  }

  /**
   * Get all orders (admin only)
   * @returns Promise with all orders
   */
  async getAdminOrders(): Promise<OrderResponseDto[]> {
    const res = await httpService.get<OrderResponseDto[]>(this.ADMIN_BASE)
    return (res as any).data ?? res
  }

  /**
   * Update the status of an order (admin only)
   * @param id - Order ID
   * @param dto - New status payload
   * @returns Promise with updated order
   */
  async updateOrderStatus(id: number, dto: UpdateOrderStatusDto): Promise<OrderResponseDto> {
    const res = await httpService.put<OrderResponseDto>(`${this.ADMIN_BASE}/${id}/status`, dto)
    return (res as any).data ?? res
  }
}

export const orderService = new OrderService()
