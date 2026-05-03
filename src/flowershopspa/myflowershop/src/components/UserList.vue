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
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">ID</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Name</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Email</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Phone</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Registered</th>
            <th class="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100">
          <!-- Empty State -->
          <tr v-if="users.length === 0">
            <td colspan="7" class="px-6 py-12 text-center text-gray-400">
              <svg class="mx-auto h-10 w-10 mb-3 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M17 20h5v-2a4 4 0 00-4-4H6a4 4 0 00-4 4v2h5M12 12a4 4 0 100-8 4 4 0 000 8z" />
              </svg>
              No users found.
            </td>
          </tr>

          <!-- Data Rows -->
          <tr
            v-for="user in users"
            :key="user.id"
            class="hover:bg-gray-50 transition"
          >
            <td class="px-6 py-4 text-sm text-gray-500">{{ user.id }}</td>
            <td class="px-6 py-4">
              <button
                @click="$emit('view-user', user)"
                class="text-sm font-medium text-blue-700 hover:underline focus:outline-none"
              >
                {{ user.name }}
              </button>
            </td>
            <td class="px-6 py-4 text-sm text-gray-700">{{ user.email }}</td>
            <td class="px-6 py-4 text-sm text-gray-700">{{ user.phone }}</td>
            <td class="px-6 py-4">
              <span :class="statusBadgeClass(user.status)" class="px-2 py-1 rounded-full text-xs font-semibold">
                {{ user.status }}
              </span>
            </td>
            <td class="px-6 py-4 text-sm text-gray-500">{{ formatDate(user.creationDate) }}</td>
            <td class="px-6 py-4">
              <div class="flex items-center gap-2">
                <!-- Approve (Pending) -->
                <button
                  v-if="user.status === 'Pending'"
                  @click="changeStatus(user, 'Active')"
                  :disabled="actionLoadingId === user.id"
                  class="px-3 py-1 bg-green-600 text-white text-xs rounded-lg hover:bg-green-700 disabled:opacity-50 transition"
                >
                  Approve
                </button>

                <!-- Suspend (Active) -->
                <button
                  v-if="user.status === 'Active'"
                  @click="changeStatus(user, 'Inactive')"
                  :disabled="actionLoadingId === user.id"
                  class="px-3 py-1 bg-red-600 text-white text-xs rounded-lg hover:bg-red-700 disabled:opacity-50 transition"
                >
                  Suspend
                </button>

                <!-- Reactivate (Inactive) -->
                <button
                  v-if="user.status === 'Inactive'"
                  @click="changeStatus(user, 'Active')"
                  :disabled="actionLoadingId === user.id"
                  class="px-3 py-1 bg-blue-600 text-white text-xs rounded-lg hover:bg-blue-700 disabled:opacity-50 transition"
                >
                  Reactivate
                </button>

                <!-- View -->
                <button
                  @click="$emit('view-user', user)"
                  class="px-3 py-1 bg-gray-100 text-gray-700 text-xs rounded-lg hover:bg-gray-200 transition"
                >
                  View
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="flex items-center justify-between px-6 py-4 border-t border-gray-100 bg-gray-50">
        <button
          @click="goToPage(currentPage - 1)"
          :disabled="currentPage <= 1"
          class="px-4 py-2 text-sm bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-40 transition"
        >
          Previous
        </button>
        <span class="text-sm text-gray-600">Page {{ currentPage }} of {{ totalPages }}</span>
        <button
          @click="goToPage(currentPage + 1)"
          :disabled="currentPage >= totalPages"
          class="px-4 py-2 text-sm bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-40 transition"
        >
          Next
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { userService } from '@/services/user.service'
import type { UserResponseDto } from '@/models/user'

const emit = defineEmits<{
  (e: 'view-user', user: UserResponseDto): void
}>()

const PAGE_SIZE = 20

const users = ref<UserResponseDto[]>([])
const isLoading = ref(false)
const actionLoadingId = ref<number | null>(null)
const currentPage = ref(1)
const totalPages = ref(1)

const filters = [
  { label: 'All', value: '' },
  { label: 'Pending', value: 'Pending' },
  { label: 'Active', value: 'Active' },
  { label: 'Suspended', value: 'Inactive' }
]
const activeFilter = ref('')

const statusBadgeClass = (status: string): string => {
  switch (status) {
    case 'Pending':
      return 'bg-yellow-100 text-yellow-800'
    case 'Active':
      return 'bg-green-100 text-green-800'
    case 'Inactive':
      return 'bg-red-100 text-red-800'
    default:
      return 'bg-gray-100 text-gray-700'
  }
}

const formatDate = (dateStr: string): string => {
  const d = new Date(dateStr)
  return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
}

const loadUsers = async () => {
  isLoading.value = true
  try {
    const result = await userService.getUsers(
      currentPage.value,
      PAGE_SIZE,
      activeFilter.value || undefined
    )
    users.value = result.items
    totalPages.value = Math.ceil(result.totalCount / result.pageSize) || 1
  } catch (err) {
    console.error('Error loading users:', err)
  } finally {
    isLoading.value = false
  }
}

const setFilter = (value: string) => {
  activeFilter.value = value
  currentPage.value = 1
  loadUsers()
}

const goToPage = (page: number) => {
  if (page < 1 || page > totalPages.value) return
  currentPage.value = page
  loadUsers()
}

const changeStatus = async (user: UserResponseDto, newStatus: string) => {
  actionLoadingId.value = user.id
  try {
    const updated = await userService.updateUserStatus(user.id, newStatus)
    users.value = users.value.map(u =>
      u.id === user.id ? { ...u, status: updated.status } : u   
    )
  } catch (err) {
    console.error('Error updating customer status:', err)
  } finally {
    actionLoadingId.value = null
  }
}

onMounted(() => {
  loadUsers()
})
</script>
