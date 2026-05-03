<template>
  <div v-if="totalPages > 1" class="flex items-center justify-between mt-8">
    <!-- Summary -->
    <p class="text-sm text-gray-600">
      Showing {{ startItem }}–{{ endItem }} of {{ totalCount }} products
    </p>

    <!-- Controls -->
    <div class="flex items-center gap-1">
      <!-- Previous -->
      <button
        @click="emit('update:page', currentPage - 1)"
        :disabled="currentPage <= 1"
        class="px-3 py-1.5 text-sm border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
        aria-label="Previous page"
      >
        &laquo; Prev
      </button>

      <!-- Page numbers -->
      <template v-for="p in visiblePages" :key="p">
        <span
          v-if="p === '...'"
          class="px-2 py-1.5 text-sm text-gray-400 select-none"
        >…</span>
        <button
          v-else
          @click="emit('update:page', p as number)"
          :class="[
            'px-3 py-1.5 text-sm border rounded-md transition-colors',
            p === currentPage
              ? 'bg-pink-600 border-pink-600 text-white font-semibold'
              : 'border-gray-300 hover:bg-gray-50 text-gray-700'
          ]"
          :aria-current="p === currentPage ? 'page' : undefined"
        >
          {{ p }}
        </button>
      </template>

      <!-- Next -->
      <button
        @click="emit('update:page', currentPage + 1)"
        :disabled="currentPage >= totalPages"
        class="px-3 py-1.5 text-sm border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
        aria-label="Next page"
      >
        Next &raquo;
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  currentPage: number
  pageSize: number
  totalCount: number
}

const props = defineProps<Props>()
const emit = defineEmits<{
  (e: 'update:page', page: number): void
}>()

const totalPages = computed(() => Math.ceil(props.totalCount / props.pageSize))

const startItem = computed(() =>
  props.totalCount === 0 ? 0 : (props.currentPage - 1) * props.pageSize + 1
)
const endItem = computed(() =>
  Math.min(props.currentPage * props.pageSize, props.totalCount)
)

/** Build visible page range with ellipsis for large page counts. */
const visiblePages = computed((): (number | '...')[] => {
  const total = totalPages.value
  const current = props.currentPage

  if (total <= 7) {
    return Array.from({ length: total }, (_, i) => i + 1)
  }

  const pages: (number | '...')[] = [1]

  if (current > 3) pages.push('...')

  const start = Math.max(2, current - 1)
  const end = Math.min(total - 1, current + 1)
  for (let i = start; i <= end; i++) pages.push(i)

  if (current < total - 2) pages.push('...')

  pages.push(total)
  return pages
})
</script>
