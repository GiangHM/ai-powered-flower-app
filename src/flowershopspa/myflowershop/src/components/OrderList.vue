<template>
  <div>
    <!-- Filter Bar -->
    <div class="flex items-center gap-2 mb-6">
      <button
        v-for="f in filters"
        :key="f.value"
        @click="setFilter(f.value)"
        :class="[
          'px-4 py-2 rounded-lg text-sm font-medium transition',
          activeFilter === f.value
            ? 'bg-blue-700 text-white'
            : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
        ]"
      >
        {{ f.label }}
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="flex justify-center items-center py-16">
      <svg class="animate-spin h-8 w-8 text-blue-600" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z" />
      </svg>
    </div>

    <!-- Table -->
    <div v-else class="bg-white rounded-xl shadow overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Order ID</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Customer</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Items</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Total</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Date</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100">
          <!-- Empty State -->
          <tr v-if="filteredOrders.length === 0">
            <td colspan="7" class="px-6 py-12 text-center text-gray-400">
              <svg class="mx-auto h-10 w-10 mb-3 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
              </svg>
              No orders found.
            </td>
          </tr>

          <!-- Data Rows -->
          <tr
            v-for="order in filteredOrders"
            :key="order.id"
            class="hover:bg-gray-50 transition"
          >
            <td class="px-6 py-4 text-sm font-medium text-gray-900">#{{ order.id }}</td>
            <td class="px-6 py-4">
              <div class="text-sm font-medium text-gray-900">{{ order.deliveryName }}</div>
              <div class="text-xs text-gray-500">{{ order.deliveryEmail }}</div>
            </td>
            <td class="px-6 py-4">
              <div class="text-sm text-gray-700">
                <span v-if="order.items.length === 0" class="text-gray-400">—</span>
                <ul v-else class="space-y-0.5">
                  <li v-for="item in order.items" :key="item.id" class="text-xs text-gray-600">
                    {{ item.flowerName }} × {{ item.quantity }}
                  </li>
                </ul>
              </div>
            </td>
            <td class="px-6 py-4 text-sm font-semibold text-gray-900">
              ${{ order.totalAmount.toFixed(2) }}
            </td>
            <td class="px-6 py-4">
              <span :class="statusBadgeClass(order.status)" class="px-2 py-1 rounded-full text-xs font-semibold">
                {{ order.status }}
              </span>
            </td>
            <td class="px-6 py-4 text-sm text-gray-500">{{ formatDate(order.orderDate) }}</td>
            <td class="px-6 py-4">
              <div class="flex items-center gap-2 flex-wrap">
                <!-- Confirm (Pending) -->
                <button
                  v-if="order.status === 'Pending'"
                  @click="updateStatus(order, 'Confirmed')"
                  :disabled="actionLoadingId === order.id"
                  class="px-3 py-1 bg-blue-600 text-white text-xs rounded-lg hover:bg-blue-700 disabled:opacity-50 transition"
                >
                  Confirm
                </button>

                <!-- Ship (Confirmed) -->
                <button
                  v-if="order.status === 'Confirmed'"
                  @click="updateStatus(order, 'Shipped')"
                  :disabled="actionLoadingId === order.id"
                  class="px-3 py-1 bg-indigo-600 text-white text-xs rounded-lg hover:bg-indigo-700 disabled:opacity-50 transition"
                >
                  Ship
                </button>

                <!-- Deliver (Shipped) -->
                <button
                  v-if="order.status === 'Shipped'"
                  @click="updateStatus(order, 'Delivered')"
                  :disabled="actionLoadingId === order.id"
                  class="px-3 py-1 bg-green-600 text-white text-xs rounded-lg hover:bg-green-700 disabled:opacity-50 transition"
                >
                  Deliver
                </button>

                <!-- Cancel (Pending or Confirmed) -->
                <button
                  v-if="order.status === 'Pending' || order.status === 'Confirmed'"
                  @click="updateStatus(order, 'Cancelled')"
                  :disabled="actionLoadingId === order.id"
                  class="px-3 py-1 bg-red-600 text-white text-xs rounded-lg hover:bg-red-700 disabled:opacity-50 transition"
                >
                  Cancel
                </button>

                <!-- Spinner while saving -->
                <svg
                  v-if="actionLoadingId === order.id"
                  class="animate-spin h-4 w-4 text-blue-600"
                  fill="none"
                  viewBox="0 0 24 24"
                >
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z" />
                </svg>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { orderService } from '@/services/order.service'
import type { OrderResponseDto } from '@/models/order'

const orders = ref<OrderResponseDto[]>([])
const isLoading = ref(false)
const actionLoadingId = ref<number | null>(null)

const filters = [
  { label: 'All', value: '' },
  { label: 'Pending', value: 'Pending' },
  { label: 'Confirmed', value: 'Confirmed' },
  { label: 'Shipped', value: 'Shipped' },
  { label: 'Delivered', value: 'Delivered' },
  { label: 'Cancelled', value: 'Cancelled' },
]
const activeFilter = ref('')

const filteredOrders = computed(() => {
  if (!activeFilter.value) return orders.value
  return orders.value.filter(o => o.status === activeFilter.value)
})

const statusBadgeClass = (status: string): string => {
  switch (status) {
    case 'Pending':   return 'bg-yellow-100 text-yellow-800'
    case 'Confirmed': return 'bg-blue-100 text-blue-800'
    case 'Shipped':   return 'bg-indigo-100 text-indigo-800'
    case 'Delivered': return 'bg-green-100 text-green-800'
    case 'Cancelled': return 'bg-red-100 text-red-800'
    default:          return 'bg-gray-100 text-gray-700'
  }
}

const formatDate = (dateStr: string): string => {
  const d = new Date(dateStr)
  return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
}

const setFilter = (value: string) => {
  activeFilter.value = value
}

const loadOrders = async () => {
  isLoading.value = true
  try {
    orders.value = await orderService.getAdminOrders()
  } catch (err) {
    console.error('Error loading orders:', err)
  } finally {
    isLoading.value = false
  }
}

const updateStatus = async (order: OrderResponseDto, newStatus: string) => {
  actionLoadingId.value = order.id
  try {
    const updated = await orderService.updateOrderStatus(order.id, { status: newStatus })
    orders.value = orders.value.map(o => o.id === order.id ? { ...o, status: updated.status } : o)
  } catch (err) {
    console.error('Error updating order status:', err)
  } finally {
    actionLoadingId.value = null
  }
}

onMounted(() => {
  loadOrders()
})
</script>
