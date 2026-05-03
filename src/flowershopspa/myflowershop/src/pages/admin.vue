<template>
  <div class="flex h-screen bg-gray-100">
    <!-- Sidebar -->
    <aside class="w-64 bg-blue-700 text-white shadow-lg">
      <div class="p-6 border-b border-blue-600">
        <div class="text-center">
          <div class="text-2xl font-bold mb-1">Logo</div>
          <div class="text-sm text-blue-100">Flower Admin</div>
        </div>
      </div>
      
      <nav class="mt-6">
        <div class="px-4 py-3">
          <h3 class="text-xs font-semibold text-blue-200 uppercase tracking-wider mb-4">Management</h3>
          <ul class="space-y-2">
            <li>
              <a
                href="#"
                @click.prevent="activeSection = 'products'"
                :class="[
                  'flex items-center px-4 py-2 rounded-lg font-medium transition',
                  activeSection === 'products' ? 'bg-blue-600 text-white' : 'text-blue-100 hover:bg-blue-600'
                ]"
              >
                <svg class="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
                Product List
              </a>
            </li>
            <li>
              <a
                href="#"
                @click.prevent="activeSection = 'users'"
                :class="[
                  'flex items-center px-4 py-2 rounded-lg font-medium transition',
                  activeSection === 'users' ? 'bg-blue-600 text-white' : 'text-blue-100 hover:bg-blue-600'
                ]"
              >
                <svg class="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M17 20h5v-2a4 4 0 00-4-4H6a4 4 0 00-4 4v2h5M12 12a4 4 0 100-8 4 4 0 000 8z" />
                </svg>
                Users
              </a>
            </li>
            <li>
              <a
                href="#"
                @click.prevent="activeSection = 'orders'"
                :class="[
                  'flex items-center px-4 py-2 rounded-lg font-medium transition',
                  activeSection === 'orders' ? 'bg-blue-600 text-white' : 'text-blue-100 hover:bg-blue-600'
                ]"
              >
                <svg class="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                </svg>
                Orders
              </a>
            </li>
          </ul>
        </div>
      </nav>
    </aside>

    <!-- Main Content -->
    <main class="flex-1 overflow-auto">
      <div class="bg-white shadow">
        <div class="px-8 py-6">
          <h1 class="text-2xl font-bold text-gray-900">
            {{ sectionTitle }}
          </h1>
          <p class="text-gray-600 mt-1">
            {{ sectionDescription }}
          </p>
        </div>
      </div>

      <div class="p-8">
        <!-- Products Section -->
        <template v-if="activeSection === 'products'">
          <FlowerList
            v-if="!showCreateView"
            :flowers="flowers"
            :isLoading="isLoading"
            @create-new="showCreateView = true"
            @toggle-status="toggleStatus"
            @edit-flower="editFlower"
            @view-flower="viewFlower"
            @delete-flower="confirmDelete"
          />

          <CreateFlower
            v-else
            @close="handleCreateClose"
          />
        </template>

        <!-- Users Section -->
        <template v-if="activeSection === 'users'">
          <UserList @view-user="openUserDetail" />
        </template>

        <!-- Orders Section -->
        <template v-if="activeSection === 'orders'">
          <OrderList />
        </template>
      </div>
    </main>

    <!-- Delete Confirmation Modal -->
    <div
      v-if="showDeleteModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
    >
      <div class="bg-white rounded-lg p-6 max-w-sm">
        <h3 class="text-lg font-bold mb-4">Delete Product</h3>
        <p class="text-gray-600 mb-6">
          Are you sure you want to delete <strong>{{ flowerToDelete?.name }}</strong>? This action cannot be undone.
        </p>
        <div class="flex gap-3 justify-end">
          <button
            @click="showDeleteModal = false"
            class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
          >
            Cancel
          </button>
          <button
            @click="deleteFlower"
            class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition"
          >
            Delete
          </button>
        </div>
      </div>
    </div>

    <!-- User Detail Modal -->
    <UserDetail
      :user="selectedUser"
      @close="selectedUser = null"
      @update="handleUserUpdate"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { flowerService } from '@/services/flower.service'
import type { FlowerAdminResponse } from '@/models/flowers/flower'
import type { UserResponseDto } from '@/models/user'
import CreateFlower from '@/components/NewFlower.vue'
import FlowerList from '@/components/FlowerList.vue'
import UserList from '@/components/UserList.vue'
import UserDetail from '@/components/UserDetail.vue'
import OrderList from '@/components/OrderList.vue'

const activeSection = ref<'products' | 'users' | 'orders'>('products')

const sectionTitle = computed(() => {
  if (activeSection.value === 'products') return 'Product Management'
  if (activeSection.value === 'users') return 'User Management'
  return 'Order Management'
})

const sectionDescription = computed(() => {
  if (activeSection.value === 'products') return 'Manage flower products'
  if (activeSection.value === 'users') return 'Manage user accounts'
  return 'View and update order statuses'
})

const isLoading = ref(false)
const showCreateView = ref(false)
const showDeleteModal = ref(false)
const flowerToDelete = ref<FlowerAdminResponse | null>(null)
const selectedUser = ref<UserResponseDto | null>(null)

const flowers = ref<FlowerAdminResponse[]>([])

const loadFlowers = async () => {
  isLoading.value = true
  try {
    const response = await flowerService.getFlowerList()
    flowers.value = response.data.map((flower: any) => ({
      ...flower
    }))
  } catch (error) {
    console.error('Error loading flowers:', error)
  } finally {
    isLoading.value = false
  }
}

const toggleStatus = async (flower: FlowerAdminResponse) => {
  const newStatus = !flower.status
  try {
    await flowerService.updateFlowerStatus(flower.id, newStatus)
    flower.status = newStatus
  } catch (error) {
    console.error('Error updating status:', error)
  }
}

const editFlower = (flower: FlowerAdminResponse) => {
  console.log('Edit flower:', flower)
}

const viewFlower = (flower: FlowerAdminResponse) => {
  console.log('View flower detail:', flower)
}

const confirmDelete = (flower: FlowerAdminResponse) => {
  flowerToDelete.value = flower
  showDeleteModal.value = true
}

const deleteFlower = async () => {
  if (!flowerToDelete.value) return
  
  try {
    await flowerService.deleteFlower(flowerToDelete.value.id)
    flowers.value = flowers.value.filter((f) => {
        return f.id !== flowerToDelete.value!.id
    })
    showDeleteModal.value = false
    flowerToDelete.value = null
  } catch (error) {
    console.error('[v0] Error deleting flower:', error)
  }
}

const handleCreateClose = () => {
  showCreateView.value = false
  loadFlowers() // Reload the list after creating
}

const openUserDetail = (user: UserResponseDto) => {
  selectedUser.value = user
}

const handleUserUpdate = (updated: UserResponseDto) => {
  selectedUser.value = updated
}

onMounted(() => {
  loadFlowers()
})
</script>
