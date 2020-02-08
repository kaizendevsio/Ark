@extends('frontend.layouts.app')

@section('content')

    <section class="gry-bg py-4 profile">
        <div class="container">
            <div class="row cols-xs-space cols-sm-space cols-md-space">
                <div class="col-lg-3 d-none d-lg-block">
                    @if(Auth::user()->user_type == 'seller')
                        @include('frontend.inc.seller_side_nav')
                    @elseif(Auth::user()->user_type == 'customer')
                        @include('frontend.inc.customer_side_nav')
                    @endif
                </div>

                <div class="col-lg-9">
                    <div class="main-content">
                        <!-- Page title -->
                        <div class="page-title">
                            <div class="row align-items-center">
                                <div class="col-md-6 col-12">
                                    <h2 class="heading heading-6 text-capitalize strong-600 mb-0">
                                        {{__('Your Enterprise')}}
                                    </h2>
                                </div>
                                <div class="col-md-6 col-12">
                                    <div class="float-md-right">
                                        <ul class="breadcrumb">
                                            <li><a href="{{ route('home') }}">{{__('Home')}}</a></li>
                                            <li><a href="{{ route('dashboard') }}">{{__('Dashboard')}}</a></li>
                                            <li class="active"><a href="{{ route('profile') }}">{{__('Your Affiliates')}}</a></li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <form class="" action="{{ route('customer.profile.update') }}" method="POST" enctype="multipart/form-data">
                            @csrf
                            <div class="form-box bg-white mt-4">
                                <div class="form-box-title px-3 py-2">
                                    {{__('Summary')}}
                                </div>
                                <div class="form-box-content p-3">
                                 
                                </div>
                            </div>
                            <div class="form-box bg-white mt-4">
                                <div class="form-box-title px-3 py-2">
                                    {{__('Direct Affiliates')}}
                                </div>
                                <div class="form-box-content p-3">
									<div class="card no-border mt-4" style="margin-top: 6px!important;">
										<div>
											<table class="table table-sm table-hover table-responsive-md">
												<thead>
													<tr>
														<th>{{__('#')}}</th>
														<th>{{__('Date')}}</th>
														<th>{{__('Name')}}</th>
														<th>{{__('Account Package')}}</th>
														<th>{{__('Status')}}</th>
														<th>{{__('Options')}}</th>
													</tr>
												</thead>
												<tbody></tbody>
											</table>
										</div>
									</div>

									<div class="text-right mt-4">
										<button type="submit" class="btn btn-styled btn-base-1  col-sm-12  col-lg-3">{{__('View Genealogy')}}</button>
									</div>
                                </div>
                            </div>

							<div class="form-box bg-white mt-4">
								<div class="form-box-title px-3 py-2">
									{{__('Product Commission')}}
								</div>
								<div class="form-box-content p-3">
									<div class="card no-border mt-4" style="margin-top: 6px!important;">
										<div>
											<table class="table table-sm table-hover table-responsive-md">
												<thead>
													<tr>
														<th>{{__('#')}}</th>
														<th>{{__('Date')}}</th>
														<th>{{__('User')}}</th>
														<th>{{__('Amount')}}</th>
														<th>{{__('Payment Status')}}</th>
														<th>{{__('Options')}}</th>
													</tr>
												</thead>
												<tbody></tbody>
											</table>
										</div>
									</div>
                                </div>
							</div>

                           
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </section>

@endsection
