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
              <a href="#" class="flex items-center px-4 py-2 rounded-lg bg-blue-600 text-white font-medium">
                <svg class="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
                Product List
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
          <h1 class="text-2xl font-bold text-gray-900">Product Management</h1>
          <p class="text-gray-600 mt-1">Manage flower products</p>
        </div>
      </div>

      <div class="p-8">
        <!-- Top Controls -->
        <div class="flex justify-between items-center mb-6">
          <div class="relative">
            <input
              v-model="searchText"
              type="text"
              placeholder="Search by Text or ID"
              class="px-4 py-2 border border-gray-300 rounded-lg w-96 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <svg class="w-5 h-5 absolute right-3 top-2.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </div>
          
          <button
            @click="showCreateModal = true"
            class="px-6 py-2 bg-blue-700 text-white rounded-lg font-medium hover:bg-blue-800 transition flex items-center gap-2"
          >
            <span>+ New Flower</span>
          </button>
        </div>

        <!-- Table -->
        <div class="bg-white rounded-lg shadow overflow-hidden">
          <div v-if="isLoading" class="p-8 text-center text-gray-500">
            Loading products...
          </div>
          
          <div v-else-if="filteredFlowers.length === 0" class="p-8 text-center text-gray-500">
            No products found
          </div>

          <table v-else class="w-full">
            <thead class="bg-gray-50 border-b border-gray-200">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">
                  <input type="checkbox" class="rounded" />
                </th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">ID</th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Flower Name</th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Category Name</th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Status</th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="flower in filteredFlowers"
                :key="flower.id"
                class="border-b border-gray-200 hover:bg-gray-50 transition"
              >
                <td class="px-6 py-4">
                  <input type="checkbox" class="rounded" />
                </td>
                <td class="px-6 py-4">
                  <span class="text-blue-600 font-medium">{{ flower.id }}</span>
                </td>
                <td class="px-6 py-4 font-medium text-gray-900">{{ flower.name }}</td>
                <td class="px-6 py-4 text-gray-600">{{ flower.categoryName }}</td>
                <td class="px-6 py-4">
                  <button
                    @click="toggleStatus(flower)"
                    :class="[
                      'px-3 py-1 rounded-full text-xs font-semibold transition',
                      flower.status === true
                        ? 'bg-green-100 text-green-800 hover:bg-green-200'
                        : 'bg-gray-100 text-gray-800 hover:bg-gray-200'
                    ]"
                  >
                    {{ flower.status === true ? '✓ Active' : '✗ Inactive' }}
                  </button>
                </td>
                <td class="px-6 py-4">
                  <div class="flex gap-2">
                     <button
                      @click="editFlower(flower)"
                      class="px-3 py-1 text-sm text-blue-600 hover:bg-blue-50 rounded transition"
                    >
                      View
                    </button>
                    <button
                      @click="editFlower(flower)"
                      class="px-3 py-1 text-sm text-blue-600 hover:bg-blue-50 rounded transition"
                    >
                      Edit
                    </button>
                    <button
                      @click="confirmDelete(flower)"
                      class="px-3 py-1 text-sm text-red-600 hover:bg-red-50 rounded transition"
                    >
                      Delete
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <div class="mt-6 flex items-center justify-between">
          <div class="text-sm text-gray-600">
            Showing {{ filteredFlowers.length }} of {{ flowers.length }} products
          </div>
          <div class="flex gap-2">
            <button class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50">Previous</button>
            <button class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50">Next</button>
          </div>
        </div>
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

     <!-- Create Flower Modal -->
    <CreateFlowerModal
      :isOpen="showCreateModal"
      @close="showCreateModal = false"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { flowerService } from '@/services/flower.service'
import type { CreateFlowerFormDto, FlowerAdminResponse } from '@/models/flowers/flower'
import CreateFlowerModal from '@/components/NewFlowerModal.vue'


const searchText = ref('')
const isLoading = ref(false)
const showCreateModal = ref(false)
const showDeleteModal = ref(false)
const flowerToDelete = ref<FlowerAdminResponse | null>(null)

const flowers = ref<FlowerAdminResponse[]>([])

const filteredFlowers = computed(() => {
  if (!searchText.value.trim()) {
    return flowers.value
  }

  const query = searchText.value.toLowerCase().trim()
  
  return flowers.value.filter((flower: { id: { toString: () => string | string[] }; name: string; categoryName: string }) => {
    const matchId = flower.id.toString().includes(query)
    const matchName = flower.name.toLowerCase().includes(query)
    const matchCategory = flower.categoryName.toLowerCase().includes(query)
    
    return matchId || matchName || matchCategory
  })
})

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
    flowers.value = flowers.value.filter((f: { id: any }) => {
        return f.id !== flowerToDelete.value!.id
    })
    showDeleteModal.value = false
    flowerToDelete.value = null
  } catch (error) {
    console.error('[v0] Error deleting flower:', error)
  }
}



onMounted(() => {
  loadFlowers()
})
</script>
