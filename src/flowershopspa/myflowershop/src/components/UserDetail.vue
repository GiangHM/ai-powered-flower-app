<template>
  <div
    v-if="user"
    class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50"
    @click.self="$emit('close')"
  >
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] flex flex-col overflow-hidden">
      <!-- Header -->
      <div class="flex items-center justify-between px-6 py-4 border-b border-gray-100">
        <h2 class="text-xl font-bold text-gray-900">User Details</h2>
        <button
          @click="$emit('close')"
          class="text-gray-400 hover:text-gray-600 transition"
          aria-label="Close"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Tabs -->
      <div class="flex border-b border-gray-100 px-6">
        <button
          @click="activeTab = 'details'"
          :class="[
            'py-3 px-4 text-sm font-medium border-b-2 transition',
            activeTab === 'details'
              ? 'border-blue-600 text-blue-700'
              : 'border-transparent text-gray-500 hover:text-gray-700'
          ]"
        >
          Details
        </button>
        <button
          @click="switchToOrders"
          :class="[
            'py-3 px-4 text-sm font-medium border-b-2 transition',
            activeTab === 'orders'
              ? 'border-blue-600 text-blue-700'
              : 'border-transparent text-gray-500 hover:text-gray-700'
          ]"
        >
          Orders
        </button>
      </div>

      <!-- Body -->
      <div class="flex-1 overflow-y-auto px-6 py-5">

        <!-- DETAILS TAB -->
        <div v-if="activeTab === 'details'" class="space-y-5">
          <!-- Read-only fields -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <p class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-1">ID</p>
              <p class="text-sm text-gray-800">{{ user.id }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-1">Email</p>
              <p class="text-sm text-gray-800">{{ user.email }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-1">Status</p>
              <span :class="statusBadgeClass(user.status)" class="px-2 py-1 rounded-full text-xs font-semibold">
                {{ user.status }}
              </span>
            </div>
            <div>
              <p class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-1">Email Verified</p>
              <p class="text-sm text-gray-800">{{ user.emailVerified ? 'Yes' : 'No' }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-1">Role</p>
              <p class="text-sm text-gray-800">{{ user.role ?? '—' }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-1">Registered</p>
              <p class="text-sm text-gray-800">{{ formatDate(user.creationDate) }}</p>
            </div>
          </div>

          <hr class="border-gray-100" />

          <!-- Editable Form -->
          <div>
            <h3 class="text-sm font-semibold text-gray-700 mb-4">Edit Profile</h3>
            <div class="space-y-3">
              <div>
                <label class="block text-xs font-medium text-gray-500 mb-1">Name</label>
                <input
                  v-model="editForm.name"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
                />
              </div>
              <div>
                <label class="block text-xs font-medium text-gray-500 mb-1">Phone</label>
                <input
                  v-model="editForm.phone"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
                />
              </div>
              <div>
                <label class="block text-xs font-medium text-gray-500 mb-1">Delivery Address</label>
                <input
                  v-model="editForm.deliveryAddress"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
                />
              </div>
            </div>

            <div class="flex items-center gap-3 mt-4">
              <button
                @click="saveChanges"
                :disabled="isSaving"
                class="px-4 py-2 bg-blue-700 text-white text-sm rounded-lg hover:bg-blue-800 disabled:opacity-50 transition"
              >
                {{ isSaving ? 'Saving…' : 'Save Changes' }}
              </button>
              <span v-if="saveSuccess" class="text-sm text-green-600 font-medium">Saved!</span>
              <span v-if="saveError" class="text-sm text-red-600">{{ saveError }}</span>
            </div>
          </div>
        </div>

        <!-- ORDERS TAB -->
        <div v-else-if="activeTab === 'orders'">
          <div v-if="ordersLoading" class="flex justify-center py-10">
            <svg class="animate-spin h-7 w-7 text-blue-600" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z" />
            </svg>
          </div>

          <div v-else-if="orders.length === 0" class="text-center py-10 text-gray-400 text-sm">
            No orders found for this user.
          </div>

          <table v-else class="min-w-full divide-y divide-gray-200 text-sm">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-4 py-2 text-left text-xs font-semibold text-gray-500 uppercase">ID</th>
                <th class="px-4 py-2 text-left text-xs font-semibold text-gray-500 uppercase">Date</th>
                <th class="px-4 py-2 text-left text-xs font-semibold text-gray-500 uppercase">Status</th>
                <th class="px-4 py-2 text-left text-xs font-semibold text-gray-500 uppercase">Total</th>
                <th class="px-4 py-2 text-left text-xs font-semibold text-gray-500 uppercase">Items</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-100">
              <tr v-for="order in orders" :key="order.id" class="hover:bg-gray-50">
                <td class="px-4 py-3 text-gray-700">#{{ order.id }}</td>
                <td class="px-4 py-3 text-gray-600">{{ formatDate(order.orderDate) }}</td>
                <td class="px-4 py-3">
                  <span class="px-2 py-0.5 rounded-full text-xs font-semibold bg-blue-100 text-blue-800">
                    {{ order.status }}
                  </span>
                </td>
                <td class="px-4 py-3 text-gray-700">${{ order.totalAmount.toFixed(2) }}</td>
                <td class="px-4 py-3 text-gray-600">{{ order.items.length }}</td>
              </tr>
            </tbody>
          </table>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { userService } from '@/services/user.service'
import type { UserResponseDto, OrderResponseDto, UpdateUserDto } from '@/models/user'

const props = defineProps<{
  user: UserResponseDto | null
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'update', user: UserResponseDto): void
}>()

const activeTab = ref<'details' | 'orders'>('details')
const orders = ref<OrderResponseDto[]>([])
const ordersLoading = ref(false)
const isSaving = ref(false)
const saveSuccess = ref(false)
const saveError = ref('')

const editForm = ref<UpdateUserDto>({
  name: '',
  phone: '',
  deliveryAddress: ''
})

const statusBadgeClass = (status: string): string => {
  switch (status) {
    case 'Pending':  return 'bg-yellow-100 text-yellow-800'
    case 'Active':   return 'bg-green-100 text-green-800'
    case 'Inactive': return 'bg-red-100 text-red-800'
    default:         return 'bg-gray-100 text-gray-700'
  }
}

const formatDate = (dateStr: string): string => {
  const d = new Date(dateStr)
  return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
}

// Reset state when the user prop changes
watch(
  () => props.user,
  (newUser) => {
    activeTab.value = 'details'
    orders.value = []
    saveSuccess.value = false
    saveError.value = ''
    if (newUser) {
      editForm.value = {
        name: newUser.name,
        phone: newUser.phone,
        deliveryAddress: newUser.deliveryAddress ?? ''
      }
    }
  },
  { immediate: true }
)

const switchToOrders = async () => {
  activeTab.value = 'orders'
  if (!props.user) return
  if (orders.value.length > 0) return
  ordersLoading.value = true
  try {
    orders.value = await userService.getUserOrders(props.user.id)
  } catch (err) {
    console.error('Error loading orders:', err)
  } finally {
    ordersLoading.value = false
  }
}

const saveChanges = async () => {
  if (!props.user) return
  isSaving.value = true
  saveSuccess.value = false
  saveError.value = ''
  try {
    const updated = await userService.updateUser(props.user.id, editForm.value)
    emit('update', updated)
    saveSuccess.value = true
    setTimeout(() => { saveSuccess.value = false }, 3000)
  } catch (err: any) {
    saveError.value = err?.message ?? 'Failed to save.'
  } finally {
    isSaving.value = false
  }
}
</script>
