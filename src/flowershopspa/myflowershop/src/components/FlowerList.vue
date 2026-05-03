<template>
  <div>
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
        @click="$emit('create-new')"
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
                @click="$emit('toggle-status', flower)"
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
                  @click="$emit('view-flower', flower)"
                  class="px-3 py-1 text-sm text-blue-600 hover:bg-blue-50 rounded transition"
                >
                  View
                </button>
                <button
                  @click="$emit('edit-flower', flower)"
                  class="px-3 py-1 text-sm text-blue-600 hover:bg-blue-50 rounded transition"
                >
                  Edit
                </button>
                <button
                  @click="$emit('delete-flower', flower)"
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
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import type { FlowerAdminResponse } from '@/models/flowers/flower'

const props = defineProps<{
  flowers: FlowerAdminResponse[]
  isLoading: boolean
}>()

defineEmits<{
  'create-new': []
  'toggle-status': [flower: FlowerAdminResponse]
  'edit-flower': [flower: FlowerAdminResponse]
  'view-flower': [flower: FlowerAdminResponse]
  'delete-flower': [flower: FlowerAdminResponse]
}>()

const searchText = ref('')

const filteredFlowers = computed(() => {
  if (!searchText.value.trim()) {
    return props.flowers
  }

  const query = searchText.value.toLowerCase().trim()
  
  return props.flowers.filter((flower) => {
    const matchId = flower.id.toString().includes(query)
    const matchName = flower.name.toLowerCase().includes(query)
    const matchCategory = flower.categoryName.toLowerCase().includes(query)
    
    return matchId || matchName || matchCategory
  })
})
</script>