<script type="text/javascript">
    function confirm_modal(delete_url)
    {
        jQuery('#confirm-delete').modal('show', {backdrop: 'static'});
        document.getElementById('delete_link').setAttribute('href' , delete_url);
    }
</script>

<div class="modal fade" id="confirm-delete" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">

            <div class="modal-header">
                
                <h4 class="modal-title" id="myModalLabel"><?php echo e(__('Confirmation')); ?></h4>
            </div>

            <div class="modal-body">
                <p><?php echo e(__('Delete confirmation message')); ?></p>
            </div>

            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal"><?php echo e(__('Cancel')); ?></button>
                <a id="delete_link" class="btn btn-danger btn-ok"><?php echo e(__('Delete')); ?></a>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="maintenance-update" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
	<div class="modal-dialog modal-lg modal-dialog-centered">
		<div class="modal-content">

			<img src="<?php echo e(asset('uploads/banners/ARK-WEB-7.png')); ?>" alt="" style="width:100%" />
			
		</div>
	</div>
</div>

<?php /**PATH C:\Projects\PHP\Ark\resources\views/frontend/partials/modal.blade.php ENDPATH**/ ?>