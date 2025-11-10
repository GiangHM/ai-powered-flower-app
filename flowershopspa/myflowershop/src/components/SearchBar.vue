<template>
  <div class="mb-8">
    <div class="relative max-w-2xl mx-auto flex gap-2">
      <div class="relative flex-1">
        <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
          <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
        </div>
        
        <input
          type="text"
          :value="modelValue"
          @input="$emit('update:modelValue', $event.target.value)"
          @keyup.enter="handleSearch"
          placeholder="Search flowers..."
          class="w-full pl-12 pr-12 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-pink-500 focus:border-transparent outline-none transition-all"
        />        
      </div> 

      <!-- Added search button -->
      <button
        @click="handleSearch"
        :disabled="!modelValue.trim()"
        class="px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
      >
        {{ isSearching ? 'Searching...' : 'Search' }}
      </button>
    </div>
    
    <!-- Checkbox for Semantic Search -->
    <div class="max-w-2xl mx-auto mt-3">
      <label class="flex items-center cursor-pointer">
        <input
          type="checkbox"
          :checked="useSemanticSearch"
          @change="$emit('update:useSemanticSearch', $event.target.checked)"
          class="w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded focus:ring-blue-500 focus:ring-2 cursor-pointer"
        />
        <span class="ml-2 text-sm text-gray-700">Use Semantic Search</span>
      </label>
    </div>
    
    <p class="text-center text-gray-600 mt-3">
      {{ resultsCount }} {{ resultsCount === 1 ? 'product' : 'products' }} found
    </p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const { modelValue, useSemanticSearch, resultsCount } = defineProps({
  modelValue: {
    type: String,
    required: true
  },
  useSemanticSearch: {
    type: Boolean,
    required: true
  },
  resultsCount: {
    type: Number,
    required: true
  }
})

const emit = defineEmits(['update:modelValue', 'update:useSemanticSearch', 'search'])

const isSearching = ref(false)

const handleSearch = async () => {
  if (!modelValue.trim()) return
  emit('search', { query: modelValue, useSemanticSearch })
}
</script>
