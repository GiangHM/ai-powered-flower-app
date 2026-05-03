<template>
  <div class="flex flex-col gap-3">
    <div class="aspect-square rounded-lg overflow-hidden bg-gray-100">
      <img
        :src="imageUrl"
        :alt="name"
        class="w-full h-full object-cover"
        @error="onImageError"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  image: string
  name: string
}

const props = defineProps<Props>()

const imageUrl = computed(() => {
  if (!props.image) return 'https://placehold.co/600x600?text=No+Image'
  if (props.image.startsWith('http://') || props.image.startsWith('https://')) return props.image
  return `/src/assets/images/${props.image}`
})

function onImageError(event: Event) {
  const img = event.target as HTMLImageElement
  img.src = 'https://placehold.co/600x600?text=No+Image'
}
</script>
