<script setup>
defineProps({
  show: Boolean
});

const emit = defineEmits(["close"]);
</script>

<template>
  <transition name="fade">
    <div class="modal-overlay" v-if="show" @click="emit('close')">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3><slot name="header">Domyślny tytuł</slot></h3>
          <button type="button" class="btn-close" aria-label="Close" @click="emit('close')"></button>
        </div>
        <div class="modal-body">
          <slot>Domyślna treść modala</slot>
        </div>
        <div class="modal-footer">
          <slot name="footer">
            
          </slot>
        </div>
      </div>
    </div>
  </transition>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100; 
}
.modal-content {
  background: rgb(0, 0, 0);
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  max-width:60vw;
  position: relative;
  z-index:101;
}
.close-btn {
  background: none;
  border: none;
  font-size: 20px;
  cursor: pointer; 
}
</style>
